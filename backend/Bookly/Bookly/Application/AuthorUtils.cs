using System.Text.RegularExpressions;

namespace Bookly.Application;

public static partial class AuthorUtils
{
    public static HashSet<string> RetrievePossibleAuthorNamesFromAuthor(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return [];
        if (name.Any(s => s is '.' or ',' or ';')) return [name.Trim()];

        var words = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var results = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        switch (words.Length)
        {
            case 1:
                results.Add(words[0]);
                break;
            case 2:
                results.Add($"{words[0]} {words[1]}");
                results.Add($"{words[1]} {words[0]}");
                results.Add($"{words[0]} {words[1][0]}.");
                results.Add($"{words[0][0]}. {words[1]}");
                results.Add($"{words[1]} {words[0][0]}.");
                results.Add($"{words[1][0]}. {words[0]}");
                break;
            case 3:
                results.Add($"{words[0]} {words[1]} {words[2]}");
                results.Add($"{words[0][0]}.{words[1][0]}. {words[2]}");
                results.Add($"{words[2]} {words[0][0]}.{words[1][0]}.");
                results.Add($"{words[2]} {words[0][0]}.");
                break;
            default:
                results.Add(name.Trim());
                break;
        }

        return results;
    }

    public static string AuthorNameToBestFormat(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return name;
        name = RemoveSpacesRegex().Replace(name, @".$1.");
        var hasSymbols = name.Any(s => s is '.' or ',' or ';');
        if (hasSymbols) return name.Trim();
        var words = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Length switch
        {
            2 when words[0].Length > 0 && words[1].Length > 0 
                => $"{words[1]} {char.ToUpper(words[0][0])}.",
            
            3 when words[0].Length > 0 && words[1].Length > 0 && words[2].Length > 0 
                => $"{words[2]} {char.ToUpper(words[0][0])}.{char.ToUpper(words[1][0])}.",

            _ => name.Trim()
        };
    }

    [GeneratedRegex(@"\.\s+([А-ЯA-Z])\.", RegexOptions.IgnoreCase)]
    private static partial Regex RemoveSpacesRegex();
}