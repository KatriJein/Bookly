using Bookly.Application.Handlers.BookCollections;
using Bookly.Application.Handlers.Passwords;
using Bookly.Application.Mappers;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core;
using Core.Dto.User;
using Core.Exceptions;
using Core.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Auth;

public class RegistrationHandler(IMediator mediator, BooklyDbContext booklyDbContext, IConfiguration configuration) : IRequestHandler<RegistrationCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(RegistrationCommand request, CancellationToken cancellationToken)
    {
        var conflictingUser = await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Login.Value == request.RegistrationRequestDto.Login ||
            u.Email.Value == request.RegistrationRequestDto.Email.ToLower(),  cancellationToken);
        if (conflictingUser is not null) return Result<AuthResponseDto>.Failure("Логин или почтовый адрес уже используются");
        var passwordHash = await mediator.Send(new HashPasswordCommand(request.RegistrationRequestDto.Password), cancellationToken);
        if (passwordHash.IsFailure) return Result<AuthResponseDto>.Failure(passwordHash.Error);
        var createUserDto = new CreateUserDto(request.RegistrationRequestDto.Login, request.RegistrationRequestDto.Email, passwordHash.Value);
        var user = User.Create(createUserDto);
        if (user.IsFailure)  return Result<AuthResponseDto>.Failure(user.Error);
        var entry = await booklyDbContext.Users.AddAsync(user.Value, cancellationToken);
        try
        {
            await mediator.Publish(new UserCreatedEvent(user.Value.Id), cancellationToken);
        }
        catch (Exception e)
        {
            return Result<AuthResponseDto>.Failure(e.Message);
        }
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        var claims = new Dictionary<string, string>()
        {
            { "UserId", user.Value.Id.ToString() },
            { "Login", user.Value.Login.Value }
        };
        var accessToken = JwtTokenGenerator.GenerateToken(claims, configuration);
        return Result<AuthResponseDto>.Success(UserMapper.MapUserToAuthResponseDto(entry.Entity, "", accessToken.Value));
    }
}

public record RegistrationCommand(RegistrationRequestDto RegistrationRequestDto) : IRequest<Result<AuthResponseDto>>;