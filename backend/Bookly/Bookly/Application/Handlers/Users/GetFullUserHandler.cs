using Bookly.Application.Handlers.Files;
using Bookly.Application.Mappers;
using Bookly.Application.Services.Files;
using Bookly.Infrastructure;
using Core.Dto.File;
using Core.Dto.User;
using Core.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Bookly.Application.Handlers.Users;

public class GetFullUserHandler(IMediator mediator, BooklyDbContext booklyDbContext, IOptionsSnapshot<BooklyOptions> booklyOptions)
    : IRequestHandler<GetFullUserQuery, GetFullUserDto?>
{
    public async Task<GetFullUserDto?> Handle(GetFullUserQuery request, CancellationToken cancellationToken)
    {
        var user = await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null) return null;
        var getPresignedUrlDto = new GetObjectPresinedUrlDto(booklyOptions.Value.BooklyFilesStorageBucketName, user.AvatarKey);
        var presignedUrl = await mediator.Send(new GetPresignedUrlQuery(getPresignedUrlDto), cancellationToken);
        return UserMapper.MapUserToFullDto(user, presignedUrl);
    }
}

public record GetFullUserQuery(Guid UserId) : IRequest<GetFullUserDto?>;