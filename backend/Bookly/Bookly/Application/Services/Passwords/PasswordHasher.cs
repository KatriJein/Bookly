using Core;

namespace Bookly.Application.Services.Passwords;

public class PasswordHasher : IPasswordHasher
{
    public Result<string> HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) return Result<string>.Failure("Указан пустой пароль");
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        return Result<string>.Success(hashedPassword);
    }

    public bool Verify(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword)) return false;
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}