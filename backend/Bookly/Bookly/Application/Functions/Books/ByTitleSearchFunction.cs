using Bookly.Domain.Models;
using Core.Interfaces;

namespace Bookly.Application.Functions.Books;

public class ByTitleSearchFunction(string? substring) : ISearchFunction<Book>
{
    public IQueryable<Book> Apply(IQueryable<Book> items)
    {
        return substring is null ? items : items.Where(i => i.Title.ToLower().Contains(substring.ToLower()));
    }
}