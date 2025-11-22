using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core.Dto.Genre;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Genres;

public class GetAllGenresHandler(BooklyDbContext booklyDbContext) : IRequestHandler<GetAllGenresQuery, List<GetGenreDto>>
{
    public async Task<List<GetGenreDto>> Handle(GetAllGenresQuery request, CancellationToken cancellationToken)
    {
        var genres = await booklyDbContext.Genres.ToListAsync(cancellationToken);
        var mappedGenres = genres
            .Select(g => new GetGenreDto(g.Id, g.Name, g.DisplayName))
            .ToList();
        return mappedGenres;
    }
}

public record GetAllGenresQuery() : IRequest<List<GetGenreDto>>;