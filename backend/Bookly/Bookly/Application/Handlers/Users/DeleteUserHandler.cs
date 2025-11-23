using Bookly.Infrastructure;
using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Users;

public class DeleteUserHandler(BooklyDbContext booklyDbContext) : IRequestHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        await booklyDbContext.Users.Where(u => u.Id == request.UserId).ExecuteDeleteAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteUserCommand(Guid UserId) : IRequest<Result>;