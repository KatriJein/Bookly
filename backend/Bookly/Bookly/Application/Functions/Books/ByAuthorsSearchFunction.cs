using Bookly.Domain.Models;
using Core.Interfaces;

namespace Bookly.Application.Functions.Books;

public class ByAuthorsSearchFunction(string[]? authors) : ISearchFunction<Book>
{
    public IQueryable<Book> Apply(IQueryable<Book> items)
    {
        if (authors is null) return items;
        var authorsHashset = authors.Select(a => a.ToLower()).ToHashSet();
        return items.Where(i => i.Authors.Any(a => authorsHashset.Contains(a.Name.ToLower())));
    }
}