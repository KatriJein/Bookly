namespace Core.ValueObjects;

public record Email : IValueObject
{
    public string Value { get; init; }

    private Email(string value)
    {
        Value = value;
    }
    
    public static Result<Email> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return Result<Email>.Failure("Указана пустая почта");
        if (!Regexes.EmailRegex().IsMatch(email)) return Result<Email>.Failure("Почта не соответствует требуемому формату");
        var value = new Email(email.ToLower());
        return Result<Email>.Success(value);
    }
}