using Bookly.Application.Handlers.Files;
using Bookly.Application.Mappers;
using Bookly.Infrastructure;
using Core.Dto.File;
using Core.Dto.User;
using Core.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Bookly.Application.Handlers.Users;

public class GetShortUserHandler(IMediator mediator, BooklyDbContext booklyDbContext, IOptionsSnapshot<BooklyOptions> booklyOptions)
    : IRequestHandler<GetShortUserQuery, GetShortUserDto?>
{
    public async Task<GetShortUserDto?> Handle(GetShortUserQuery request, CancellationToken cancellationToken)
    {
        var user = await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null) return null;
        var getPresignedUrlDto = new GetObjectPresinedUrlDto(booklyOptions.Value.BooklyFilesStorageBucketName, user.AvatarKey);
        var presignedUrl = await mediator.Send(new GetPresignedUrlQuery(getPresignedUrlDto), cancellationToken);
        return UserMapper.MapUserToShortDto(user, presignedUrl);
    }
}

public record GetShortUserQuery(Guid UserId) : IRequest<GetShortUserDto?>;