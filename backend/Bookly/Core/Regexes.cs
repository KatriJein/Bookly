using System.Text.RegularExpressions;

namespace Core;

public static partial class Regexes
{
    [GeneratedRegex(@"^[a-zA-Z0-9._-]{3,20}$")]
    public static partial Regex LoginRegex();

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    public static partial Regex EmailRegex();

    [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d@$!%*?&_]{8,32}$")]
    public static partial Regex PasswordRegex();
}