using Bookly.Infrastructure;
using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.UserSubscriptions;

public class UnsubscribeFromUserHandler(BooklyDbContext booklyDbContext) : IRequestHandler<UnsubscribeFromUserCommand, Result>
{
    public async Task<Result> Handle(UnsubscribeFromUserCommand request, CancellationToken cancellationToken)
    {
        var userExists = await booklyDbContext.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken: cancellationToken);
        if (!userExists)
            return Result.Failure("Указан несуществующий пользователь");
        var followingUserExists = await booklyDbContext.Users.AnyAsync(u => u.Id == request.FollowingUserId, cancellationToken: cancellationToken);
        if (!followingUserExists)
            return Result.Failure("Указан несуществующий пользователь");
        await booklyDbContext.UserSubscriptions.Where(us => us.UserId == request.UserId &&
                                                            us.FollowingUserId == request.FollowingUserId)
            .ExecuteDeleteAsync(cancellationToken);
        return Result.Success();
    }
}

public record UnsubscribeFromUserCommand(Guid UserId, Guid FollowingUserId) : IRequest<Result>;