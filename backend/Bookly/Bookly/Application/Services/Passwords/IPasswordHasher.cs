using Core;

namespace Bookly.Application.Services.Passwords;

public interface IPasswordHasher
{
    Result<string> HashPassword(string password);
    bool Verify(string password, string hashedPassword);
}