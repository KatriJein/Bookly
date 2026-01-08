using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.UserSubscriptions;

public class SubscribeToUserHandler(BooklyDbContext booklyDbContext) : IRequestHandler<SubscribeToUserCommand, Result>
{
    public async Task<Result> Handle(SubscribeToUserCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == request.FollowingUserId)
            return Result.Failure("Нельзя оформить подписку на самого себя");
        var userExists = await booklyDbContext.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken: cancellationToken);
        if (!userExists)
            return Result.Failure("Указан несуществующий пользователь");
        var followingUserExists = await booklyDbContext.Users.AnyAsync(u => u.Id == request.FollowingUserId, cancellationToken: cancellationToken);
        if (!followingUserExists)
            return Result.Failure("Указан несуществующий пользователь");
        var hasSubscription = await booklyDbContext.UserSubscriptions.AnyAsync(u => u.UserId == request.UserId 
            && u.FollowingUserId == request.FollowingUserId, cancellationToken: cancellationToken);
        if (hasSubscription)
            return Result.Success();
        var subscription = UserSubscription.Create(request.UserId, request.FollowingUserId);
        booklyDbContext.UserSubscriptions.Add(subscription);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record SubscribeToUserCommand(Guid UserId, Guid FollowingUserId) : IRequest<Result>;