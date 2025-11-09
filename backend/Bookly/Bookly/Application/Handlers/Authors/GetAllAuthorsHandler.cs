using Bookly.Infrastructure;
using Core.Dto.Author;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Authors;

public class GetAllAuthorsHandler(BooklyDbContext booklyDbContext) : IRequestHandler<GetAllAuthorsQuery, List<GetAuthorDto>>
{
    public async Task<List<GetAuthorDto>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
    {
        var authors = await booklyDbContext.Authors.ToListAsync(cancellationToken);
        var mappedAuthors = authors
            .Select(a => new GetAuthorDto(a.Id, a.Name, a.DisplayName))
            .ToList();
        return mappedAuthors;
    }
}

public record GetAllAuthorsQuery() : IRequest<List<GetAuthorDto>>;