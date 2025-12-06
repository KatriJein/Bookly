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
    private const double MinRequiredSimilarity = 0.48;
    private const double BestSimilaritySide = 0.9;
    private const double GoodSimilaritySide = 0.75;
    
    public async Task<Result<List<GetShortBookDto>>> Handle(GetUserInterestBooksQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId is null) return Result<List<GetShortBookDto>>.Failure("Несуществующий пользователь");
        var user = await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null) return Result<List<GetShortBookDto>>.Failure("Несуществующий или неавторизованный пользователь");
        var userGenrePreferences = await booklyDbContext.UserGenrePreferences
            .Where(p => p.UserId == request.UserId && (p.PreferenceType == PreferenceType.Liked || p.PreferenceType == PreferenceType.Neutral))
            .Select(p => p.GenreId)
            .ToHashSetAsync(cancellationToken);
        var userAuthorPreferences = await booklyDbContext.UserAuthorPreferences
            .Where(p => p.UserId == request.UserId && (p.PreferenceType == PreferenceType.Liked ||  p.PreferenceType == PreferenceType.Neutral))
            .Select(p => p.AuthorId)
            .ToHashSetAsync(cancellationToken);
        if (userGenrePreferences.Count == 0 && userAuthorPreferences.Count == 0) return Result<List<GetShortBookDto>>.Success([]);
        var possibleBooks = await GetBooksInUserInterests(userGenrePreferences, userAuthorPreferences,
            user.VolumeSizePreference, user.AgeCategory, cancellationToken);
        var relevantPossibleBooks = await mediator.Send(new ExcludeIrrelevantBooksAndEnrichRelevantWithDataCommand(possibleBooks,
            request.UserId, request.BookSimpleSearchSettingsDto), cancellationToken);
        var rearrangedBooks = relevantPossibleBooks.OrderBySimilarityWeightAndShuffle(BestSimilaritySide, GoodSimilaritySide);
        var booksDto = rearrangedBooks.Select(BookMapper.MapBookToShortBookDto).ToList();
        return Result<List<GetShortBookDto>>.Success(booksDto);
    }

    private async Task<List<Book>> GetBooksInUserInterests(HashSet<Guid> genrePreferences, HashSet<Guid> authorPreferences,
        VolumeSizePreference? volumeSizePreference, AgeCategory? ageCategory, CancellationToken cancellationToken)
    {
        var possibleInterestingBooks = await booklyDbContext.Books
            .AsNoTracking()
            .Where(b => b.Genres.Any(g => genrePreferences.Contains(g.Id)) || b.Authors.Any(a => authorPreferences.Contains(a.Id)))
            .Include(b => b.Genres)
            .Include(b => b.Authors)
            .ToListAsync(cancellationToken);
        foreach (var book in possibleInterestingBooks)
        {
            var bookGenres = book.Genres.Select(g => g.Id).ToHashSet();
            var bookAuthors = book.Authors.Select(a => a.Id).ToHashSet();
            var userSuitableGenrePreferences = genrePreferences.Intersect(bookGenres).ToHashSet();
            var userSuitableAuthorPreferences = authorPreferences.Intersect(bookAuthors).ToHashSet();
            var bookVolumeSize = BookUtils.GetVolumeSizeDependingOnPagesCount(book.PageCount);
            var bookAgeCategory = BookUtils.GetAgeCategoryDependingOnAgeRestriction(book.AgeRestriction);
            var genresJaccard = Utils.CalculateJaccard(userSuitableGenrePreferences, bookGenres);
            var authorsJaccard = Utils.CalculateJaccard(userSuitableAuthorPreferences, bookAuthors);
            var volumeValue = bookVolumeSize == volumeSizePreference ? 1 : 0;
            var ageCategoryValue = bookAgeCategory == ageCategory ? 1 : 0;
            var similarity = SimilarityScores.Scores[BookSimilarityType.ByAuthor] * authorsJaccard
                + SimilarityScores.Scores[BookSimilarityType.ByGenre] * genresJaccard
                + SimilarityScores.Scores[BookSimilarityType.ByAgeRestriction] * ageCategoryValue
                + SimilarityScores.Scores[BookSimilarityType.ByVolumeSize] * volumeValue;
            book.SimilarityWeight = Utils.NormalizeSimilarity(similarity, SimilarityScores.MaxPossibleScoreForUserInterestBooksHandler);
        }
        return possibleInterestingBooks.Where(b => b.SimilarityWeight >= MinRequiredSimilarity).ToList();
    }
}

public record GetUserInterestBooksQuery(BookSimpleSearchSettingsDto BookSimpleSearchSettingsDto, Guid? UserId)
    : IRequest<Result<List<GetShortBookDto>>>;