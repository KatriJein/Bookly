using Bookly.Infrastructure;
using Core.Dto.BookCollection;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.BookCollections;

public class GetBookCollectionsBriefInfoHandler(BooklyDbContext booklyDbContext) : IRequestHandler<GetBookCollectionsBriefInfoQuery, List<GetShortBookCollectionDto>>
{
    public async Task<List<GetShortBookCollectionDto>> Handle(GetBookCollectionsBriefInfoQuery request, CancellationToken cancellationToken)
    {
        var collections = await booklyDbContext.BookCollections
            .Where(bc => bc.UserId == request.UserId)
            .OrderByDescending(bc => bc.IsStatic)
            .ThenBy(c => c.Title)
            .ToListAsync(cancellationToken);
        return collections
            .Select(c => new GetShortBookCollectionDto(c.Id, c.Title, c.UserId))
            .ToList();
    }
}

public record GetBookCollectionsBriefInfoQuery(Guid UserId) : IRequest<List<GetShortBookCollectionDto>>;