using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Rateable;

public class CalculateAverageRatingHandler<T>() : IRequestHandler<CalculateAverageRatingQuery<T>, double>
where T : RateableEntity
{
    public async Task<double> Handle(CalculateAverageRatingQuery<T> request, CancellationToken cancellationToken)
    {
        var collectionsRatings = request.Entities
            .Where(bc => bc.RatingsCount > 0)
            .Select(bc => bc.Rating);
        var averageCollectionsRating = !collectionsRatings.Any() 
            ? 0 
            : await collectionsRatings.AverageAsync(cancellationToken);
        return averageCollectionsRating;
    }
}

public record CalculateAverageRatingQuery<T>(DbSet<T> Entities) : IRequest<double> where T : RateableEntity;