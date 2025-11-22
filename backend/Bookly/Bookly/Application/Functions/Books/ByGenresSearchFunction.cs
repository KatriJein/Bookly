using Bookly.Domain.Models;
using Core.Interfaces;

namespace Bookly.Application.Functions.Books;

public class ByGenresSearchFunction(string[]? genres) : ISearchFunction<Book>
{
    public IQueryable<Book> Apply(IQueryable<Book> items)
    {
        if (genres is null) return items;
        var genresHashset = genres.Select(g => g.ToLower()).ToHashSet();
        return items.Where(i => i.Genres.Any(g => genresHashset.Contains(g.DisplayName.ToLower())));
    }
}