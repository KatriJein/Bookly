using Bookly.Application.Handlers.Passwords;
using Bookly.Application.Services.Passwords;
using Bookly.Infrastructure;
using Core;
using Core.Dto.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Users;

public class UpdateUserPasswordHandler(IMediator mediator, BooklyDbContext booklyDbContext, IPasswordHasher passwordHasher)
    : IRequestHandler<UpdateUserPasswordCommand, Result>
{
    public async Task<Result> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null) return Result.Failure("Указан Id несуществующего пользователя");
        var oldPasswordCorrect = passwordHasher.Verify(request.UpdatePasswordDto.OldPassword, user.PasswordHash);
        if (!oldPasswordCorrect) return Result.Failure("Указан неверный текущий пароль");
        var passwordHash = await mediator.Send(new HashPasswordCommand(request.UpdatePasswordDto.NewPassword), cancellationToken);
        if (passwordHash.IsFailure) return Result.Failure(passwordHash.Error);
        user.SetPasswordHash(passwordHash.Value);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record UpdateUserPasswordCommand(Guid UserId, UpdatePasswordDto UpdatePasswordDto) : IRequest<Result>;