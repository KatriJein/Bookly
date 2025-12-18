using Core.Enums;

namespace Core.Dto.Recommendation;

public record RecommendationDto(Guid BookId, RecommendationStatus RecommendationStatus);