using Core.Enums;

namespace Core.Payloads;

public record RespondedToRecommendationPreferenceActionPayload(RecommendationStatus RecommendationStatus) : IPrerefenceActionPayload;