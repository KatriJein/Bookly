namespace Core.Dto.UserSubscription;

public record GetUserSubscriptionDto(Guid FollowingId, string FollowingName, string FollowingAvatar);