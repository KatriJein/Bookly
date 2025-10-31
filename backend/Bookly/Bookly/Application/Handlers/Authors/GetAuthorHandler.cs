using Bookly.Domain.Models;
using Bookly.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Authors;

public class GetAuthorHandler(BooklyDbContext booklyDbContext) : IRequestHandler<GetAuthorQuery, Author?>
{
    public async Task<Author?> Handle(GetAuthorQuery request, CancellationToken cancellationToken)
    {
        var authorName = request.Name;
        var possibleAuthorNames = AuthorUtils
            .RetrievePossibleAuthorNamesFromAuthor(authorName)
            .Select(n => n.ToLower())
            .ToHashSet();
        var author = await booklyDbContext.Authors.FirstOrDefaultAsync(a => possibleAuthorNames.Contains(a.Name.ToLower()),
                cancellationToken);
        return author;
    }
}

public record GetAuthorQuery(string Name) : IRequest<Author?>;