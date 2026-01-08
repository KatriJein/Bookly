using Core.Enums;

namespace Core.Dto.ImpulseSituations;

public record ImpulseSituationFullDto(string Situation, List<string> PreferredGenres, List<string>? AvoidGenres,
    VolumeSizePreference Length, string Note);