using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core.Data;
using Core.Dto.Genre;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace Bookly.Application.Services;

public class GenresSeedService(ILogger logger, IServiceScopeFactory serviceScopeFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var booklyDbContext =  scope.ServiceProvider.GetRequiredService<BooklyDbContext>();
        var genresCount = await booklyDbContext.Genres.CountAsync(cancellationToken);
        if (genresCount > 0)
        {
            logger.Information("Таблица жанров не пуста. Пропуск добавления данных");
            return;
        }
        var genres = GenresData.BestGenres
            .Select(g => Genre.Create(new CreateGenreDto(g.Key, g.Value)).Value);
        await booklyDbContext.Genres.AddRangeAsync(genres, cancellationToken);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        logger.Information("Жанры добавлены в таблицу БД");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
    }
}