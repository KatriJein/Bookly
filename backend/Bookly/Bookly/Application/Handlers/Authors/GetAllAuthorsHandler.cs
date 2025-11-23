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
        var authorNameLowered = request.AuthorSearchSettingsDto.Name?.ToLower();
        var mappedAuthors = authors
            .Select(a => new GetAuthorDto(a.Id, a.Name, a.DisplayName))
            .Where(a => authorNameLowered == null || a.DisplayName.ToLower().Contains(authorNameLowered))
            .ToList();
        return mappedAuthors;
    }
}

public record GetAllAuthorsQuery(AuthorSearchSettingsDto AuthorSearchSettingsDto) : IRequest<List<GetAuthorDto>>;