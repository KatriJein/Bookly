using Core;
using Core.Dto.Recommendation;
using Core.Enums;

namespace Bookly.Domain.Models;

public class Recommendation : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public Guid BookId { get; private set; }
    public RecommendationStatus RecommendationStatus { get; private set; }

    public static Recommendation Create(RecommendationDto recommendationDto, Guid userId)
    {
        return new Recommendation()
        {
            UserId = userId,
            BookId = recommendationDto.BookId,
            RecommendationStatus = recommendationDto.RecommendationStatus
        };
    }
}