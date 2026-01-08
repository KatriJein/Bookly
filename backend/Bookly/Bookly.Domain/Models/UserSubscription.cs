using Core;
using Core.Dto.UserSubscription;

namespace Bookly.Domain.Models;

public class UserSubscription : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public Guid FollowingUserId { get; private set; }

    public User User { get; private set; }

    public static UserSubscription Create(Guid userId, Guid followingUserId)
    {
        return new UserSubscription()
        {
            UserId = userId,
            FollowingUserId = followingUserId
        };
    }
}