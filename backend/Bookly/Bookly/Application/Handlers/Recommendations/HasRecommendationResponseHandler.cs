using Bookly.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Recommendations;

public class HasRecommendationResponseHandler(BooklyDbContext booklyDbContext) : IRequestHandler<HasRecommendationResponseQuery, bool>
{
    public async Task<bool> Handle(HasRecommendationResponseQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId is null || request.UserId == Guid.Empty) return true;
        return await booklyDbContext.Recommendations.AnyAsync(r => r.UserId == request.UserId && r.BookId ==  request.BookId,
            cancellationToken);
    }
}

public record HasRecommendationResponseQuery(Guid BookId, Guid? UserId) : IRequest<bool>;