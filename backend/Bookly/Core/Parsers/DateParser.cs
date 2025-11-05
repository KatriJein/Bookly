using System.Globalization;
using System.Text.RegularExpressions;

namespace Core.Parsers;

public static partial class DateParser
{
    public static bool TryParseBookDate(string? input, out DateTime date)
    {
        date = default;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        input = input.Trim();
        
        if (OnlyYearRegex().IsMatch(input))
        {
            date = new DateTime(int.Parse(input), 1, 1);
            return true;
        }
        
        if (YearAndMonthRegex().IsMatch(input))
        {
            var parts = input.Split('-');
            date = new DateTime(int.Parse(parts[0]), int.Parse(parts[1]), 1);
            return true;
        }
        
        if (DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            date = parsed;
            return true;
        }
        
        return false;
    }

    [GeneratedRegex(@"^\d{4}$")]
    private static partial Regex OnlyYearRegex();
    [GeneratedRegex(@"^\d{4}-\d{2}$")]
    private static partial Regex YearAndMonthRegex();
}