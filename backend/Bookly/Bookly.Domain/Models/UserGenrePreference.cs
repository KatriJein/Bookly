using Core;
using Core.Dto.UserGenrePreference;
using Core.Enums;

namespace Bookly.Domain.Models;

public class UserGenrePreference : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public Guid GenreId { get; private set; }
    public PreferenceType PreferenceType { get; private set; }

    public static UserGenrePreference Create(UserPreferenceDto userPreferenceDto)
    {
        return new UserGenrePreference()
        {
            UserId = userPreferenceDto.UserId,
            GenreId = userPreferenceDto.EntityId,
            PreferenceType = userPreferenceDto.PreferenceType
        };
    }
}