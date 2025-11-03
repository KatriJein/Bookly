using Bookly.Application.Chains.LoginChain;
using Bookly.Application.Handlers.Files;
using Bookly.Application.Mappers;
using Bookly.Application.Services.Passwords;
using Core;
using Core.Dto.File;
using Core.Dto.User;
using Core.Options;
using MediatR;
using Microsoft.Extensions.Options;

namespace Bookly.Application.Handlers.Auth;

public class AuthenthicationHandler(IMediator mediator, ILoginChain loginChain, IPasswordHasher passwordHasher,
    IOptionsSnapshot<BooklyOptions> booklyOptions) : IRequestHandler<AuthenthicationCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(AuthenthicationCommand request, CancellationToken cancellationToken)
    {
        var user = await loginChain.FindUserByLoginAsync(request.AuthRequestDto.Login, cancellationToken);
        if (user == null) return Result<AuthResponseDto>.Failure("Не найден пользователь с указанным логином");
        var isCorrectPassword = passwordHasher.Verify(request.AuthRequestDto.Password, user.PasswordHash);
        if (!isCorrectPassword) return Result<AuthResponseDto>.Failure("Некорректный логин или пароль");
        var getPresignedUrlDto = new GetObjectPresinedUrlDto(booklyOptions.Value.BooklyFilesStorageBucketName, user.AvatarKey);
        var presignedUrl = await mediator.Send(new GetPresignedUrlQuery(getPresignedUrlDto), cancellationToken);
        return Result<AuthResponseDto>.Success(UserMapper.MapUserToAuthResponseDto(user, presignedUrl));
    }
}

public record AuthenthicationCommand(AuthRequestDto AuthRequestDto) : IRequest<Result<AuthResponseDto>>;