using Bookly.Domain.Models;
using Core.Enums;
using Core.Interfaces;

namespace Bookly.Application.Functions.Books;

public class BooksSortingFunction(BooksOrderOption? booksOrderOption) : ISortingFunction<Book>
{
    public IQueryable<Book> Apply(IQueryable<Book> items)
    {
        if (booksOrderOption is null) return items;
        return booksOrderOption switch
        {
            BooksOrderOption.ByTitleAscending => items.OrderBy(i => i.Title),
            BooksOrderOption.ByTitleDescending => items.OrderByDescending(i => i.Title),
            BooksOrderOption.ByRatingAscending => items.OrderBy(i => i.Rating),
            BooksOrderOption.ByRatingDescending => items.OrderByDescending(i => i.Rating),
            BooksOrderOption.ByDateAscending => items.OrderBy(i => i.PublishmentYear ?? int.MaxValue),
            BooksOrderOption.ByDateDescending => items.OrderByDescending(i => i.PublishmentYear ?? 0),
            BooksOrderOption.ByPageCountAscending => items.OrderBy(i => i.PageCount),
            BooksOrderOption.ByPageCountDescending => items.OrderByDescending(i => i.PageCount),
            BooksOrderOption.ByPopularityAscending => items.OrderBy(i => i.RatingsCount),
            BooksOrderOption.ByPopularityDescending => items.OrderByDescending(i => i.RatingsCount),
            _ => items
        };
    }
}