using Core.Enums;

namespace Core.Dto.Survey;

public record EntrySurveyDataDto(Guid UserId, string[] FavoriteGenres, string[] HatedGenres, string[] FavoriteAuthors,
    string[] HatedAuthors, VolumeSizePreference VolumeSizePreference, AgeCategory AgeCategory, bool HatedGenresToBlacklist = false);