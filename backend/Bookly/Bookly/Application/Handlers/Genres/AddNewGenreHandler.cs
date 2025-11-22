using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core;
using Core.Dto.Genre;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace Bookly.Application.Handlers.Genres;

public class AddNewGenreHandler(BooklyDbContext booklyDbContext, ILogger logger) : IRequestHandler<AddNewGenreCommand, Result<Genre>>
{
    public async Task<Result<Genre>> Handle(AddNewGenreCommand request, CancellationToken cancellationToken)
    {
        var genre = await 
            booklyDbContext.Genres.FirstOrDefaultAsync(g => g.Name.ToLower() == request.CreateGenreDto.Name.ToLower(), cancellationToken);
        if (genre != null)
        {
            logger.Information("Жанр {@genre} уже существует. Пропуск процедуры создания жанра", request.CreateGenreDto.Name);
            return Result<Genre>.Success(genre);
        }
        var genreRes = Genre.Create(request.CreateGenreDto);
        if (genreRes.IsFailure) return genreRes;
        var entity = await booklyDbContext.Genres.AddAsync(genreRes.Value, cancellationToken);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        logger.Information("Жанр {@genre} успешно добавлен в БД", genreRes.Value.Name);
        return Result<Genre>.Success(entity.Entity);
    }
}

public record AddNewGenreCommand(CreateGenreDto CreateGenreDto) : IRequest<Result<Genre>>;