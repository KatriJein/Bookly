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
            .OrderBySimilarityWeightAndShuffle()
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
        var simpleBooksSearchDto = new BookSimpleSearchSettingsDto(bookSearchSettingsDto.Page, bookSearchSettingsDto.Limit);
        return await mediator.Send(new ExcludeIrrelevantBooksAndEnrichRelevantWithDataCommand(suitableBooks, userId, simpleBooksSearchDto),
            cancellationToken);
    }
    
    private void FillSimilarityWeight(SimilarityDto similarityDto, Book candidate)
    {
        var candidateAuthors = candidate.Authors.Select(a => a.Name).ToHashSet();
        var candidateGenres = candidate.Genres.Select(a => a.Name).ToHashSet();
        var authorJaccard = Utils.CalculateJaccard(candidateAuthors, similarityDto.Authors);
        var genreJaccard = Utils.CalculateJaccard(candidateGenres, similarityDto.Genres);
        var candidateVolumeSize =  BookUtils.GetVolumeSizeDependingOnPagesCount(candidate.PageCount);
        candidate.SimilarityWeight += SimilarityScores.Scores[BookSimilarityType.ByAuthor] * authorJaccard;
        candidate.SimilarityWeight += SimilarityScores.Scores[BookSimilarityType.ByGenre] * genreJaccard;
        if (candidate.Language == similarityDto.Language)
            candidate.SimilarityWeight += SimilarityScores.Scores[BookSimilarityType.ByLanguage];
        if (candidate.AgeRestriction == similarityDto.AgeRestriction)
            candidate.SimilarityWeight += SimilarityScores.Scores[BookSimilarityType.ByAgeRestriction];
        if (candidateVolumeSize == similarityDto.VolumeSizePreference)
            candidate.SimilarityWeight += SimilarityScores.Scores[BookSimilarityType.ByVolumeSize];
        candidate.SimilarityWeight = Utils.NormalizeSimilarity(candidate.SimilarityWeight, SimilarityScores.MaxPossibleScoreForSimilarBooksHandler);
    }
}

public record GetSimilarBooksQuery(Guid BookId, BookSimpleSearchSettingsDto SearchSettingsDto, Guid? UserId = null) : IRequest<List<GetShortBookDto>>;

public record SimilarityDto(HashSet<string> Genres, HashSet<string> Authors, string Language, AgeRestriction AgeRestriction,
    VolumeSizePreference VolumeSizePreference);