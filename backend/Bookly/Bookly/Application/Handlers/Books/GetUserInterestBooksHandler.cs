using Bookly.Application.Mappers;
using Bookly.Domain.Models;
using Bookly.Extensions;
using Bookly.Infrastructure;
using Core;
using Core.Data;
using Core.Dto.Book;
using Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Books;

public class GetUserInterestBooksHandler(IMediator mediator, BooklyDbContext booklyDbContext) : IRequestHandler<GetUserInterestBooksQuery, Result<List<GetShortBookDto>>>
{
    private const double MinRequiredSimilarity = 0.2;
    
    public async Task<Result<List<GetShortBookDto>>> Handle(GetUserInterestBooksQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId is null) return Result<List<GetShortBookDto>>.Failure("Несуществующий пользователь");
        var user = await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null) return Result<List<GetShortBookDto>>.Failure("Несуществующий или неавторизованный пользователь");
        var userGenrePreferences = await booklyDbContext.UserGenrePreferences
            .Where(p => p.UserId == request.UserId && (p.PreferenceType == PreferenceType.Liked || p.PreferenceType == PreferenceType.Neutral))
            .ToDictionaryAsync(p => p.GenreId, p => p.Weight, cancellationToken);
        var userAuthorPreferences = await booklyDbContext.UserAuthorPreferences
            .Where(p => p.UserId == request.UserId && (p.PreferenceType == PreferenceType.Liked ||  p.PreferenceType == PreferenceType.Neutral))
            .ToDictionaryAsync(p => p.AuthorId, p => p.Weight, cancellationToken);
        if (userGenrePreferences.Count == 0 && userAuthorPreferences.Count == 0) return Result<List<GetShortBookDto>>.Success([]);
        var possibleBooks = await GetBooksInUserInterests(userGenrePreferences, userAuthorPreferences,
            user.VolumeSizePreference, user.AgeCategory, cancellationToken);
        var rearrangedBooks = RearrangeBooks(possibleBooks);
        var relevantPossibleBooks = await mediator.Send(new ExcludeIrrelevantBooksAndEnrichRelevantWithDataCommand(rearrangedBooks,
            request.UserId, request.BookSimpleSearchSettingsDto), cancellationToken);
        var booksDto = relevantPossibleBooks.Select(BookMapper.MapBookToShortBookDto).ToList();
        return Result<List<GetShortBookDto>>.Success(booksDto);
    }

    private async Task<List<Book>> GetBooksInUserInterests(Dictionary<Guid, double> genreWeights,
        Dictionary<Guid, double> authorWeigths, VolumeSizePreference? volumeSizePreference, AgeCategory? ageCategory,
        CancellationToken cancellationToken)
    {
        var genreIds = genreWeights.Keys.ToHashSet();
        var authorIds = authorWeigths.Keys.ToHashSet();
        var possibleInterestingBooks = await booklyDbContext.Books
            .AsNoTracking()
            .Where(b => b.Genres.Any(g => genreIds.Contains(g.Id)) || b.Authors.Any(a => authorIds.Contains(a.Id)))
            .Include(b => b.Genres)
            .Include(b => b.Authors)
            .ToListAsync(cancellationToken);
        foreach (var book in possibleInterestingBooks)
        {
            var bookGenres = book.Genres.Select(g => g.Id).ToHashSet();
            var bookAuthors = book.Authors.Select(a => a.Id).ToHashSet();
            var averageGenresWeightSum = CalculateAverageWeight(bookGenres, genreWeights);
            var averageAuthorsWeightSum = CalculateAverageWeight(bookAuthors, authorWeigths);
            var bookVolumeSize = BookUtils.GetVolumeSizeDependingOnPagesCount(book.PageCount);
            var bookAgeCategory = BookUtils.GetAgeCategoryDependingOnAgeRestriction(book.AgeRestriction);
            var volumeValue = bookVolumeSize == volumeSizePreference ? 1 : 0;
            var ageCategoryValue = bookAgeCategory == ageCategory ? 1 : 0;
            var similarity = SimilarityScores.Scores[BookSimilarityType.ByAuthor] * averageAuthorsWeightSum
                + SimilarityScores.Scores[BookSimilarityType.ByGenre] * averageGenresWeightSum
                + SimilarityScores.Scores[BookSimilarityType.ByAgeRestriction] * ageCategoryValue
                + SimilarityScores.Scores[BookSimilarityType.ByVolumeSize] * volumeValue;
            book.SimilarityWeight = Utils.NormalizeSimilarity(similarity, SimilarityScores.MaxPossibleScoreForUserInterestBooksHandler);
        }
        return possibleInterestingBooks.Where(b => b.SimilarityWeight >= MinRequiredSimilarity).ToList();
    }

    private List<Book> RearrangeBooks(List<Book> books)
    {
        return books
            .GroupBy(b => Math.Min(9, (int)Math.Floor(b.SimilarityWeight * 10)))
            .OrderByDescending(g => g.Key)
            .Select(g => g.OrderBy(_ => Guid.NewGuid()))
            .SelectMany(g => g)
            .ToList();
    }
    
    private static double CalculateAverageWeight(HashSet<Guid> entitiesIds, Dictionary<Guid, double> weightsTable)
    {
        var values = weightsTable
            .Where(i => entitiesIds.Contains(i.Key))
            .Select(i => i.Value)
            .DefaultIfEmpty(0)
            .ToList();
        if (values.Count != 0) return values.Average();
        return 0;
    }
}

public record GetUserInterestBooksQuery(BookSimpleSearchSettingsDto BookSimpleSearchSettingsDto, Guid? UserId)
    : IRequest<Result<List<GetShortBookDto>>>;