using Bookly.Application.Handlers.Passwords;
using Bookly.Application.Mappers;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core;
using Core.Dto.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Auth;

public class RegistrationHandler(IMediator mediator, BooklyDbContext booklyDbContext) : IRequestHandler<RegistrationCommand, Result<AuthResponseDto>>
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
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        return Result<AuthResponseDto>.Success(UserMapper.MapUserToAuthResponseDto(entry.Entity, ""));
    }
}

public record RegistrationCommand(RegistrationRequestDto RegistrationRequestDto) : IRequest<Result<AuthResponseDto>>;