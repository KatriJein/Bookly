using Bookly.Application.Services.Passwords;
using Core;
using MediatR;

namespace Bookly.Application.Handlers.Passwords;

public class HashPasswordHandler(IPasswordHasher passwordHasher) : IRequestHandler<HashPasswordCommand, Result<string>>
{
    public async Task<Result<string>> Handle(HashPasswordCommand request, CancellationToken cancellationToken)
    {
        var passwordMatchesFormat = Regexes.PasswordRegex().IsMatch(request.Password);
        if (!passwordMatchesFormat) return Result<string>.Failure("Пароль не соответствует формату");
        var passwordHash = passwordHasher.HashPassword(request.Password);
        return passwordHash;
    }
}

public record HashPasswordCommand(string Password) : IRequest<Result<string>>;