using Bookly.Application.Handlers.Files;
using Bookly.Infrastructure;
using Core.Dto.File;
using Core.Dto.UserSubscription;
using Core.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Bookly.Application.Handlers.UserSubscriptions;

public class GetUserSubscriptionsHandler(IMediator mediator, BooklyDbContext booklyDbContext,
    IOptionsSnapshot<BooklyOptions> booklyOptions) : IRequestHandler<GetUserSubscriptionsQuery, List<GetUserSubscriptionDto>>
{
    public async Task<List<GetUserSubscriptionDto>> Handle(GetUserSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId is null || request.UserId == Guid.Empty) return [];
        var userSubscriptions = await booklyDbContext.UserSubscriptions
            .Where(us => us.UserId == request.UserId)
            .Select(us => us.FollowingUserId)
            .ToHashSetAsync(cancellationToken);
        var followingUsers = await booklyDbContext.Users
            .Where(u => userSubscriptions.Contains(u.Id))
            .ToListAsync(cancellationToken);
        var dtoTasks = followingUsers.Select(async user =>
        {
            var getAvatarDto =
                new GetObjectPresinedUrlDto(booklyOptions.Value.BooklyFilesStorageBucketName, user.AvatarKey);
            var userAvatarUrl = await mediator.Send(new GetPresignedUrlQuery(getAvatarDto), cancellationToken);
            return new GetUserSubscriptionDto(user.Id, user.Login.Value, userAvatarUrl);
        }).ToList();
        var results = await Task.WhenAll(dtoTasks);
        return results.ToList();
    }
}

public record GetUserSubscriptionsQuery(Guid? UserId) : IRequest<List<GetUserSubscriptionDto>>;