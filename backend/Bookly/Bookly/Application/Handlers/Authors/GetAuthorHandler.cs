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
        var containingAuthor = await booklyDbContext.Authors.FirstOrDefaultAsync(a => a.Name.Contains(authorName), cancellationToken);
        if (containingAuthor != null) return containingAuthor;
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