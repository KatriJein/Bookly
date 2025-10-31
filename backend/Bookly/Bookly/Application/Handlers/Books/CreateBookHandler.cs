using Bookly.Application.Handlers.Authors;
using Bookly.Application.Handlers.Genres;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core;
using Core.Dto.Author;
using Core.Dto.Book;
using Core.Dto.Genre;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace Bookly.Application.Handlers.Books;

public class CreateBookHandler(IMediator mediator, BooklyDbContext booklyDbContext, ILogger logger) : IRequestHandler<CreateBookCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var existingBook = await booklyDbContext.Books.FirstOrDefaultAsync(b => b.ExternalId == request.CreateBookDto.ExternalId, cancellationToken);
        if (existingBook != null)
        {
            logger.Information("Книга {@book} (id: {@id}) уже существует. Пропуск создания книги", request.CreateBookDto.Title,
                request.CreateBookDto.ExternalId);
            return Result<Guid>.Success(existingBook.Id);
        }
        var genres = request.CreateBookDto.Genres.Select(g => g.Trim());
        var authors = request.CreateBookDto.Authors.Select(a => a.Trim());
        var genresModels = await GetGenresAsync(genres);
        var authorModels = await GetAuthorsAsync(authors);
        var book = Book.Create(request.CreateBookDto);
        if (book.IsFailure)
        {
            logger.Error("Не удалось создать книгу {@book} (id: {@id}): {@error}", request.CreateBookDto.Title,
                request.CreateBookDto.ExternalId, book.Error);
            return Result<Guid>.Failure(book.Error);
        }
        book.Value.AddGenres(genresModels);
        book.Value.AddAuthors(authorModels);
        var savedBook = await booklyDbContext.Books.AddAsync(book.Value, cancellationToken);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(savedBook.Entity.Id);
    }

    private async Task<IEnumerable<Genre>> GetGenresAsync(IEnumerable<string> genres)
    {
        return await GetValuesAsync(genres,
            async genre => await booklyDbContext.Genres.FirstOrDefaultAsync(g => g.Name.ToLower() == genre.ToLower()),
            async genre => await mediator.Send(new AddNewGenreCommand(new CreateGenreDto(genre, genre))));
    }

    private async Task<IEnumerable<Author>> GetAuthorsAsync(IEnumerable<string> authors)
    {
        return await GetValuesAsync(authors,
            async author => await mediator.Send(new GetAuthorQuery(author)),
            async author => await mediator.Send(new CreateAuthorCommand(new CreateAuthorDto(author, author))));
    }
    
    private async Task<IEnumerable<TOut>> GetValuesAsync<TOut, TIn>(IEnumerable<TIn> values, Func<TIn,
        Task<TOut?>> searchFunc, Func<TIn, Task<Result<TOut>>> createFunc)
    {
        var models = new List<TOut>();
        foreach (var value in values)
        {
            var existingModel = await searchFunc(value);
            if (existingModel != null)
            {
                models.Add(existingModel);
                continue;
            }
            var modelRes = await createFunc(value);
            if (modelRes.IsSuccess) models.Add(modelRes.Value);
        }
        return models;
    }
}

public record CreateBookCommand(CreateBookDto CreateBookDto) : IRequest<Result<Guid>>;