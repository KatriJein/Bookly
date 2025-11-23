using System.Text.RegularExpressions;

namespace Core.ValueObjects;

public record Login : IValueObject
{
    public string Value { get; init; }

    private Login(string value)
    {
        Value = value;
    }
    
    public static Result<Login> Create(string login)
    {
        if (string.IsNullOrWhiteSpace(login)) return Result<Login>.Failure("Указан пустой логин");
        if (!Regexes.LoginRegex().IsMatch(login)) return Result<Login>.Failure("Логин не соответствует требуемому формату");
        var value = new Login(login);
        return Result<Login>.Success(value);
    }
}