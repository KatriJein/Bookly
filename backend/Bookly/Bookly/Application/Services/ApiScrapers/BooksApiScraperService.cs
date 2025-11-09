using System.Globalization;
using Bookly.Application.Handlers.Books;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core;
using Core.Data;
using Core.Dto.Book;
using Core.Mappers;
using Core.Options;
using Core.Parsers;
using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Util;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ILogger = Serilog.ILogger;

namespace Bookly.Application.Services.ApiScrapers;

public class BooksApiScraperService(IRecurringJobManager recurringJobManager, IMediator mediator, BooklyDbContext booklyDbContext,
    ILogger logger, IConfiguration configuration, IOptionsSnapshot<BooklyOptions> booklyOptions) : IBooksApiScraperService
{
    private readonly IClientService _booksService = new BooksService();

    private const string LangRestrict = "ru";
    private const VolumesResource.ListRequest.PrintTypeEnum PrintType = VolumesResource.ListRequest.PrintTypeEnum.BOOKS;
    private const int BooksIndexStep = 10;
    
    [DisableConcurrentExecution(300)]
    public async Task ScrapeNextAsync()
    {
        if (!booklyOptions.Value.ShouldDoBooksScraping)
        {
            logger.Warning("Опция {@parameterName} выключена. Операция сбора книг с API отменена", nameof(booklyOptions.Value.ShouldDoBooksScraping));
            return;
        }
        var currentScrapingTaskState = await booklyDbContext.ScrapingTaskStates
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync() ?? ScrapingTaskState.Create(0, 0, 0, null);
        if (currentScrapingTaskState.CooldownUntil != null && DateTime.UtcNow < currentScrapingTaskState.CooldownUntil)
        {
            logger.Information("Пропуск следующего сбора данных с API книг: не закончился период таймаута");
            return;
        }
        var genre = GenresData.BestGenres.Keys.ElementAt(currentScrapingTaskState.CurrentGenreIndex);
        var publisher = PublishersData.BestPublishersList[currentScrapingTaskState.CurrentPublisherIndex];
        logger.Information("Выполняется шаг получения книг с API запросом жанр: {@genre}, издатель: {@publisher}, стартовый индекс: {@startIndex}",
            genre, publisher, currentScrapingTaskState.CurrentIndex);
        var volumes = await ExecuteVolumesSearchAsync(genre, publisher, currentScrapingTaskState.CurrentIndex);
        if (volumes.IsFailure)
        {
            logger.Error("Произошла ошибка при получении книг с API. Будет реализован таймаут на 1 минуту");
            var cooldownScrapingTaskState = ScrapingTaskState.Create(currentScrapingTaskState.CurrentGenreIndex,
                currentScrapingTaskState.CurrentPublisherIndex,
                currentScrapingTaskState.CurrentIndex, DateTime.UtcNow.AddMinutes(1));
            await booklyDbContext.ScrapingTaskStates.AddAsync(cooldownScrapingTaskState);
            await booklyDbContext.SaveChangesAsync();
            return;
        }
        await AddBooksAsync(volumes.Value, genre, publisher);
        logger.Information("Шаг получения книг с API запросом жанр: {@genre}, издатель: {@publisher}, стартовый индекс: {@startIndex} завершен",
            genre, publisher, currentScrapingTaskState.CurrentIndex);
        var nextScrapingTaskState = PrepareNextScrapingTaskState(volumes.Value, currentScrapingTaskState);
        if (nextScrapingTaskState is null)
        {
            logger.Information("Сбор книг с API завершен. Завершение фоновой задачи");
            recurringJobManager.RemoveIfExists(Const.BooksApiScrapingJob);
            return;
        }
        await booklyDbContext.AddAsync(nextScrapingTaskState);
        await booklyDbContext.SaveChangesAsync();
        logger.Information("Запланирован новый шаг получения книг с API");
    }

    private async Task<Result<Volumes?>> ExecuteVolumesSearchAsync(string subject, string publisher, int startIndex)
    {
        try
        {
            var volumesResource = new VolumesResource(_booksService);
            var query = $"subject:{subject} inpublisher:{publisher}";
            var request = volumesResource.List(query);
            request.StartIndex = startIndex;
            request.LangRestrict = LangRestrict;
            request.PrintType = PrintType;
            request.Key = configuration[Const.GoogleBooksApiKey];
            var volumes = await request.ExecuteAsync();
            return Result<Volumes?>.Success(volumes);
        }
        catch (Exception e)
        {
            logger.Error("Не удалось выполнить запрос к API книг: {@apiError}", e.Message);
            return Result<Volumes?>.Failure("Не удалось выполнить запрос к API книг");
        }
    }

    private async Task AddBooksAsync(Volumes? volumes, string subject, string publisher)
    {
        if (volumes is null || volumes.Items is null || volumes.Items.Count == 0)
        {
            logger.Information("Тома в запросе по жанру {@genre} и издателю {@publisher} отсутствуют", subject, publisher);
            return;
        }
        foreach (var volume in volumes.Items)
        {
            try
            {
                var volumeId = volume.Id;
                var volumeInfo = volume.VolumeInfo;
                if (!DateParser.TryParseBookDate(volumeInfo.PublishedDate, out var date)) 
                    throw new Exception($"Не удалось распарсить дату издания тома ({volumeInfo.PublishedDate})");
                if (volumeInfo.Authors.Count <= 0) throw new Exception("Отсутствуют авторы тома");
                var createBookDto = new CreateBookDto(
                    volumeInfo.Title,
                    volumeInfo.Description,
                    volumeInfo.AverageRating ?? 0,
                    volumeInfo.RatingsCount ?? 0,
                    volumeInfo.Language,
                    publisher,
                    date.Year,
                    volumeInfo.PageCount ?? 0,
                    EnumMapper.MapStringMaturityRatingToAgeRestrictionEnum(volumeInfo.MaturityRating),
                    volumeInfo.ImageLinks.Thumbnail,
                    volumeId,
                    volumeInfo.Authors.ToArray(),
                    [subject]);
                await mediator.Send(new CreateBookCommand(createBookDto));
            }
            catch (Exception e)
            {
                logger.Error("Не удалось обработать том {@volumeId}: {@volumeError}", volume.Id, e.Message);
            }
        }
    }

    private static ScrapingTaskState? PrepareNextScrapingTaskState(Volumes? volumes, ScrapingTaskState currentState)
    {
        if (volumes is not null && volumes.Items is not null && volumes.Items.Count != 0)
            return ScrapingTaskState.Create(currentState.CurrentGenreIndex, currentState.CurrentPublisherIndex,
                currentState.CurrentIndex + BooksIndexStep, null);
        var nextPublisherIndex = currentState.CurrentPublisherIndex + 1;
        if (nextPublisherIndex < PublishersData.BestPublishersList.Length)
            return ScrapingTaskState.Create(currentState.CurrentGenreIndex, nextPublisherIndex, 0, null);
        nextPublisherIndex = 0;
        var nextGenreIndex = currentState.CurrentGenreIndex + 1;
        return nextGenreIndex < GenresData.BestGenres.Count 
            ? ScrapingTaskState.Create(nextGenreIndex, nextPublisherIndex, 0, null) 
            : null;
    }
}