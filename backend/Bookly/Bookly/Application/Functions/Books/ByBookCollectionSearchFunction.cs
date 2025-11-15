using Bookly.Domain.Models;
using Core.Interfaces;

namespace Bookly.Application.Functions.Books;

public class ByBookCollectionSearchFunction(Guid? searchByCollection) : ISearchFunction<Book>
{
    public IQueryable<Book> Apply(IQueryable<Book> items)
    {
        if (searchByCollection is null) return items;
        var collectionId = searchByCollection.Value;
        return items.Where(b => b.BookCollections.Any(bc => bc.Id == collectionId));
    }
}