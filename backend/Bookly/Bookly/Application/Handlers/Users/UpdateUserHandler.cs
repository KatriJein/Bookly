using Bookly.Infrastructure;
using Core;
using Core.Dto.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Users;

public class UpdateUserHandler(BooklyDbContext booklyDbContext) : IRequestHandler<UpdateUserCommand, Result>
{
    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null) return Result.Failure("Указан Id несуществующего пользователя");
        var conflictingUser = await booklyDbContext.Users
            .FirstOrDefaultAsync(u =>
                    (request.UpdateUserDto.Login != null && u.Login.Value == request.UpdateUserDto.Login) ||
                    (request.UpdateUserDto.Email != null && u.Email.Value == request.UpdateUserDto.Email.ToLower()),
                cancellationToken);
        if (conflictingUser is not null && conflictingUser.Id != user.Id)
            return Result.Failure("Пользователь с таким логином / паролем уже существует");
        user.SetLogin(request.UpdateUserDto.Login ?? user.Login.Value);
        user.SetEmail(request.UpdateUserDto.Email ?? user.Email.Value);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record UpdateUserCommand(Guid UserId, UpdateUserDto UpdateUserDto) : IRequest<Result>;