using Bookly.Domain.Models;
using Core.Interfaces;

namespace Bookly.Application.Functions.Books;

public class ByPublisherSearchFunction(string? publisher) : ISearchFunction<Book>
{
    public IQueryable<Book> Apply(IQueryable<Book> items)
    {
        return publisher is null ? items : items.Where(i => i.Publisher.Name.ToLower() == publisher.ToLower());
    }
}