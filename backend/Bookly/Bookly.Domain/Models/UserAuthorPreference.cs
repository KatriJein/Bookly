using Core;
using Core.Dto.UserGenrePreference;
using Core.Enums;

namespace Bookly.Domain.Models;

public class UserAuthorPreference : Preference
{
    public Guid UserId { get; private set; }
    public Guid AuthorId { get; private set; }

    public static UserAuthorPreference Create(UserPreferenceDto userPreferenceDto)
    {
        var preference = new UserAuthorPreference()
        {
            UserId = userPreferenceDto.UserId,
            AuthorId = userPreferenceDto.EntityId,
            PreferenceType = userPreferenceDto.PreferenceType,
            Weight = userPreferenceDto.Weight ?? 0
        };
        preference.UpdatePreferenceType();
        return preference;
    }
}