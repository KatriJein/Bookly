using Core;
using Core.Dto.UserGenrePreference;
using Core.Enums;

namespace Bookly.Domain.Models;

public class UserAuthorPreference : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public Guid AuthorId { get; private set; }
    public PreferenceType PreferenceType { get; private set; }

    public static UserAuthorPreference Create(UserPreferenceDto userPreferenceDto)
    {
        return new UserAuthorPreference()
        {
            UserId = userPreferenceDto.UserId,
            AuthorId = userPreferenceDto.EntityId,
            PreferenceType = userPreferenceDto.PreferenceType
        };
    }
}