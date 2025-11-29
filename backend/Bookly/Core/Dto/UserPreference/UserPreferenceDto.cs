using Core.Enums;

namespace Core.Dto.UserGenrePreference;

public record UserPreferenceDto(Guid UserId, Guid EntityId, PreferenceType PreferenceType, double? Weight)
{
    public virtual bool Equals(UserPreferenceDto? other)
    {
        return other is not null && UserId == other.UserId && EntityId == other.EntityId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(UserId, EntityId);
    }
}