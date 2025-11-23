using Bookly.Domain.Models;
using Core;

namespace Bookly.Extensions;

public static class IOrderedEnumerableExtensions
{
    public static IOrderedEnumerable<Book> OrderThenByDescendingWeightedRatings(this IOrderedEnumerable<Book> items, double averageEntitiesRating)
    {
        return items
            .ThenByDescending(i =>
                ((double)i.RatingsCount / (i.RatingsCount + Const.TrustedRatingsCount)) * i.Rating +
                ((double)Const.TrustedRatingsCount / (i.RatingsCount + Const.TrustedRatingsCount)) *
                averageEntitiesRating);
    }
}