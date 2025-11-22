using Bookly.Infrastructure;
using Core.Dto.Publisher;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Publishers;

public class GetPublishersHandler(BooklyDbContext booklyDbContext) : IRequestHandler<GetPublishersQuery, List<GetPublisherDto>>
{
    public async Task<List<GetPublisherDto>> Handle(GetPublishersQuery request, CancellationToken cancellationToken)
    {
        var publishers = await booklyDbContext.Publishers.OrderBy(p => p.Name).ToListAsync(cancellationToken);
        return publishers.Select(publisher => new GetPublisherDto(publisher.Name)).ToList();
    }
}

public record GetPublishersQuery() : IRequest<List<GetPublisherDto>>;