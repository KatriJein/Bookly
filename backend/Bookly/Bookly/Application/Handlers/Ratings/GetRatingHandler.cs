using Bookly.Infrastructure;
using Core;
using Core.Dto.Rating;
using Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Ratings;

public class GetRatingHandler<T>(BooklyDbContext booklyDbContext) : IRequestHandler<GetRatingQuery<T>> where T : RateableEntity
{
    public async Task<Unit> Handle(GetRatingQuery<T> request, CancellationToken cancellationToken)
    {
        if (request.UserId is null) return Unit.Value;
        var user = await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null) return Unit.Value;
        var entitiesIds = request.Entities.Select(e => e.Id).ToHashSet();
        var ratings = await booklyDbContext.Ratings
            .Where(r => entitiesIds.Contains(r.EntityId) && r.UserId == request.UserId)
            .ToDictionaryAsync(r => r.EntityId, r => r.Value, cancellationToken);
        foreach (var entity in request.Entities)
        {
            if (ratings.TryGetValue(entity.Id, out var rating))
                entity.UserRating = rating;
        }
        return Unit.Value;
    }
}

public record GetRatingQuery<T>(List<T> Entities, Guid? UserId) : IRequest
where T : RateableEntity;