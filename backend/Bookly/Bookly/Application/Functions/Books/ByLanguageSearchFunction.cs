using Bookly.Domain.Models;
using Core.Interfaces;

namespace Bookly.Application.Functions.Books;

public class ByLanguageSearchFunction(string? language) : ISearchFunction<Book>
{
    public IQueryable<Book> Apply(IQueryable<Book> items)
    {
        return language is null 
            ? items 
            : items.Where(i => i.Language == language);
    }
}