using Core;
using Core.Dto.UserGenrePreference;
using Core.Enums;

namespace Bookly.Domain.Models;

public class UserGenrePreference : Preference
{
    public Guid UserId { get; private set; }
    public Guid GenreId { get; private set; }

    public static UserGenrePreference Create(UserPreferenceDto userPreferenceDto)
    {
        var preference = new UserGenrePreference()
        {
            UserId = userPreferenceDto.UserId,
            GenreId = userPreferenceDto.EntityId,
            PreferenceType = userPreferenceDto.PreferenceType,
            Weight = userPreferenceDto.Weight ?? 0
        };
        preference.UpdatePreferenceType();
        return preference;
    }
}