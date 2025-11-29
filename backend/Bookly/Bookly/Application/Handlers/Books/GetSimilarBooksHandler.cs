using Bookly.Application.Functions.Books;
using Bookly.Application.Handlers.Rateable;
using Bookly.Application.Handlers.Ratings;
using Bookly.Application.Mappers;
using Bookly.Domain.Models;
using Bookly.Extensions;
using Bookly.Infrastructure;
using Core.Data;
using Core.Dto.Book;
using Core.Enums;
using Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Books;

public class GetSimilarBooksHandler(IMediator mediator, BooklyDbContext booklyDbContext) : IRequestHandler<GetSimilarBooksQuery, List<GetShortBookDto>>
{
    public async Task<List<GetShortBookDto>> Handle(GetSimilarBooksQuery request, CancellationToken cancellationToken)
    {
        var book = await booklyDbContext.Books.FirstOrDefaultAsync(b => b.Id == request.BookId, cancellationToken);
        if (book is null) return [];
        await booklyDbContext.Entry(book).Collection(b => b.Authors).LoadAsync(cancellationToken);
        await booklyDbContext.Entry(book).Collection(b => b.Genres).LoadAsync(cancellationToken);
        var booksAverageRating = await mediator.Send(new CalculateAverageRatingQuery<Book>(booklyDbContext.Books),
            cancellationToken);
        var authors = book.Authors.Select(a => a.Name).ToArray();
        var genres = book.Genres.Select(a => a.Name).ToArray();
        var volumeSize = BookUtils.GetVolumeSizeDependingOnPagesCount(book.PageCount);
        var getBooksFilter = new BookSearchSettingsDto(Page: request.SearchSettingsDto.Page, Limit: request.SearchSettingsDto.Limit,
            SearchByAuthors: authors, SearchByGenres: genres, Language: book.Language, AgeRestriction: book.AgeRestriction,
            SearchByVolumeSizePreference: volumeSize);
        var suitableBooks = await GetSuitableBooks(getBooksFilter, request.UserId, cancellationToken);
        suitableBooks.RemoveAll(b => b.Id == request.BookId);
        var similarityDto = new SimilarityDto(genres.ToHashSet(), authors.ToHashSet(), book.Language, book.AgeRestriction, volumeSize);
        foreach (var suitableBook in suitableBooks)
            FillSimilarityWeight(similarityDto, suitableBook);
        return suitableBooks
            .OrderByDescending(b => b.SimilarityWeight)
            .OrderThenByDescendingWeightedRatings(booksAverageRating)
            .Select(BookMapper.MapBookToShortBookDto)
            .ToList();
    }

    private async Task<List<Book>> GetSuitableBooks(BookSearchSettingsDto bookSearchSettingsDto, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var books = booklyDbContext.Books
            .Include(b => b.Genres)
            .Include(b => b.Authors);
        var filters = new ISearchFunction<Book>[]
        {
            new ByAuthorsSearchFunction(bookSearchSettingsDto.SearchByAuthors),
            new ByGenresSearchFunction(bookSearchSettingsDto.SearchByGenres),
            new ByAgeRestrictionSearchFunction(bookSearchSettingsDto.AgeRestriction),
            new ByLanguageSearchFunction(bookSearchSettingsDto.Language),
            new ByVolumeSizeSearchFunction(bookSearchSettingsDto.SearchByVolumeSizePreference)
        };
        var suitableBooks = new List<Book>();
        foreach (var filter in filters)
        {
            var foundBooks = await books
                .ApplySearchFunctionsForItems([filter])
                .ToListAsync(cancellationToken);
            suitableBooks.AddRange(foundBooks);
        }
        suitableBooks = await mediator.Send(new ExcludeIrrelevantBooksCommand(suitableBooks, userId), cancellationToken);
        suitableBooks = suitableBooks.DistinctBy(b => b.Id)
            .Skip((bookSearchSettingsDto.Page - 1) * bookSearchSettingsDto.Limit)
            .Take(bookSearchSettingsDto.Limit)
            .ToList();
        await mediator.Send(new MarkFavoritesCommand(suitableBooks, userId), cancellationToken);
        await mediator.Send(new GetRatingQuery<Book>(suitableBooks, userId), cancellationToken);
        return suitableBooks;
    }
    
    private void FillSimilarityWeight(SimilarityDto similarityDto, Book candidate)
    {
        var candidateAuthors = candidate.Authors.Select(a => a.Name).ToHashSet();
        var candidateGenres = candidate.Genres.Select(a => a.Name).ToHashSet();
        var overlapAuthorsCount = similarityDto.Authors.Intersect(candidateAuthors).Count();
        var authorJaccard = (double)overlapAuthorsCount / (similarityDto.Authors.Count + candidateAuthors.Count - overlapAuthorsCount);
        var overlapGenresCount = similarityDto.Genres.Intersect(candidateGenres).Count();
        var genreJaccard = (double)overlapGenresCount / (similarityDto.Genres.Count + candidateGenres.Count - overlapGenresCount);
        var candidateVolumeSize =  BookUtils.GetVolumeSizeDependingOnPagesCount(candidate.PageCount);
        candidate.SimilarityWeight += SimilarityScores.Scores[BookSimilarityType.ByAuthor] * authorJaccard;
        candidate.SimilarityWeight += SimilarityScores.Scores[BookSimilarityType.ByGenre] * genreJaccard;
        if (candidate.Language == similarityDto.Language)
            candidate.SimilarityWeight += SimilarityScores.Scores[BookSimilarityType.ByLanguage];
        if (candidate.AgeRestriction == similarityDto.AgeRestriction)
            candidate.SimilarityWeight += SimilarityScores.Scores[BookSimilarityType.ByAgeRestriction];
        if (candidateVolumeSize == similarityDto.VolumeSizePreference)
            candidate.SimilarityWeight += SimilarityScores.Scores[BookSimilarityType.ByVolumeSize];
    }
}

public record GetSimilarBooksQuery(Guid BookId, BookSimpleSearchSettingsDto SearchSettingsDto, Guid? UserId = null) : IRequest<List<GetShortBookDto>>;

public record SimilarityDto(HashSet<string> Genres, HashSet<string> Authors, string Language, AgeRestriction AgeRestriction,
    VolumeSizePreference VolumeSizePreference);