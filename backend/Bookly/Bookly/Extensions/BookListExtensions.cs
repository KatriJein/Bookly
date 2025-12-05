using Bookly.Domain.Models;

namespace Bookly.Extensions;

public static class BookListExtensions
{
    public const double BestSimilarityFrom = 0.7;
    public const double GoodSimilarityFrom = 0.4;
    
    public static IList<Book> OrderBySimilarityWeightAndShuffle(this IList<Book> books)
    {
        var shuffledBooks = books.OrderBy(_ => Guid.NewGuid());
        var mostSimilarBooks = new List<Book>();
        var goodSimilarBooks = new List<Book>();
        var leastSimilarBooks = new List<Book>();
        foreach (var book in shuffledBooks)
        {
            if (book.SimilarityWeight >= BestSimilarityFrom) mostSimilarBooks.Add(book);
            else if (book.SimilarityWeight < GoodSimilarityFrom) leastSimilarBooks.Add(book);
            else goodSimilarBooks.Add(book);
        }
        return [..mostSimilarBooks, ..goodSimilarBooks, ..leastSimilarBooks];
    }
}