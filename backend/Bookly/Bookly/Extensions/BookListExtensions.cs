using Bookly.Domain.Models;

namespace Bookly.Extensions;

public static class BookListExtensions
{
    public static IList<Book> OrderBySimilarityWeightAndShuffle(this IList<Book> books, double bestSimilarityFrom,
        double goodSimilarityFrom)
    {
        var shuffledBooks = books.OrderBy(_ => Guid.NewGuid());
        var mostSimilarBooks = new List<Book>();
        var goodSimilarBooks = new List<Book>();
        var leastSimilarBooks = new List<Book>();
        foreach (var book in shuffledBooks)
        {
            if (book.SimilarityWeight >= bestSimilarityFrom) mostSimilarBooks.Add(book);
            else if (book.SimilarityWeight >= goodSimilarityFrom) goodSimilarBooks.Add(book);
            else leastSimilarBooks.Add(book);
        }
        return [..mostSimilarBooks, ..goodSimilarBooks, ..leastSimilarBooks];
    }
}