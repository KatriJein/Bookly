namespace Core.Payloads;

public record RatedPreferenceActionPayload(int Rating) : IPrerefenceActionPayload;