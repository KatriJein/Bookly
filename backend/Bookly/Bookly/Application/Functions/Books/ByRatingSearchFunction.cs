using Bookly.Domain.Models;
using Core.Interfaces;

namespace Bookly.Application.Functions.Books;

public class ByRatingSearchFunction(double? rating) : ISearchFunction<Book>
{
    public IQueryable<Book> Apply(IQueryable<Book> items)
    {
        return rating is null ? items : items.Where(i => i.Rating >= rating);
    }
}