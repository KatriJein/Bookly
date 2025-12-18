using Bookly.Application.Handlers.Rateable;
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

public class GetPossiblyLikedBooksHandler(IMediator mediator, BooklyDbContext booklyDbContext) : IRequestHandler<GetPossiblyLikedBooksQuery, Result<List<GetShortBookDto>>>
{
    private const int MaxSimilarUsers = 20;
    private const double MinRequiredSimilarity = 0.6;
    private const int TrustedGenrePreferencesCount = 3;
    private const int MaxBooksFromGenre = 10;
    
    public async Task<Result<List<GetShortBookDto>>> Handle(GetPossiblyLikedBooksQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId is null) return Result<List<GetShortBookDto>>.Failure("Раздел доступен только авторизованным пользователям");
        var currentUser = await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (currentUser is null) return Result<List<GetShortBookDto>>.Failure("Пользователь не авторизован или не существует");
        await booklyDbContext.Entry(currentUser).Collection(u => u.UserGenrePreferences).LoadAsync(cancellationToken);
        await booklyDbContext.Entry(currentUser).Collection(u => u.UserAuthorPreferences).LoadAsync(cancellationToken);
        await booklyDbContext.Entry(currentUser).Collection(u => u.BookCollections).LoadAsync(cancellationToken);
        var preferredGenres = currentUser.UserGenrePreferences
            .Where(p => p.PreferenceType is PreferenceType.Liked or PreferenceType.Neutral)
            .ToList();
        var preferredAuthors = currentUser.UserAuthorPreferences
            .Where(p => p.PreferenceType is  PreferenceType.Liked or PreferenceType.Neutral)
            .ToList();
        var similarUsers = await FindSimilarUsersAsync(currentUser, preferredGenres, preferredAuthors);
        var books = similarUsers.Count > 0
            ? await GetYouMayLikeBooksAsync(similarUsers, cancellationToken)
            : await GetBestBooksOfGenresAsync(preferredGenres, cancellationToken);
        var preparedBooks = await mediator.Send(new ExcludeIrrelevantBooksAndEnrichRelevantWithDataCommand(books,
            request.UserId, request.BookSimpleSearchSettingsDto), cancellationToken);
        var booksDto = preparedBooks.Select(BookMapper.MapBookToShortBookDto).ToList();
        return Result<List<GetShortBookDto>>.Success(booksDto);
    }
    
    private async Task<List<User>> FindSimilarUsersAsync(User currentUser, List<UserGenrePreference> genrePreferences,
        List<UserAuthorPreference> authorPreferences)
    {
        var genrePreferencesIds = genrePreferences.Select(gp => gp.GenreId);
        var authorPreferencesIds = authorPreferences.Select(a => a.AuthorId);
        var possiblySimilarUsers = await booklyDbContext.Users.Where(u =>
                u.UserAuthorPreferences.Any(p => authorPreferencesIds.Contains(p.AuthorId)) ||
                u.UserGenrePreferences.Any(p => genrePreferencesIds.Contains(p.GenreId)))
            .Where(u => u.Id != currentUser.Id)
            .Include(u => u.UserAuthorPreferences)
            .Include(u => u.UserGenrePreferences)
            .ToListAsync();
        var similarUsers = new List<User>();
        foreach (var possiblySimilarUser in possiblySimilarUsers)
        {
            var similarity = await CalculateUserSimilarityAsync(currentUser, genrePreferences, authorPreferences, possiblySimilarUser);
            if (similarity >= MinRequiredSimilarity) similarUsers.Add(possiblySimilarUser);
        }
        return similarUsers
            .OrderBy(_ => Guid.NewGuid())
            .Take(MaxSimilarUsers)
            .ToList();
    }

    private async Task<double> CalculateUserSimilarityAsync(User currentUser, List<UserGenrePreference> currentUserGenrePreferences,
        List<UserAuthorPreference> currentUserAuthorPreferences, User otherUser)
    {
        const string readBooksCollectionTitle = StaticBookCollectionsData.Read;
        var currentUserReadBookIds = await booklyDbContext.BookCollections
            .Where(bc => bc.IsStatic && bc.UserId == currentUser.Id && bc.Title == readBooksCollectionTitle)
            .SelectMany(bc => bc.Books.Select(b => b.Id))
            .ToHashSetAsync();
        var otherUserReadBookIds = await booklyDbContext.BookCollections
            .Where(bc => bc.IsStatic && bc.UserId == otherUser.Id && bc.Title == readBooksCollectionTitle)
            .SelectMany(bc => bc.Books.Select(b => b.Id))
            .ToHashSetAsync();
        var otherUserGenrePreferences = otherUser.UserGenrePreferences
            .Where(p => p.PreferenceType is PreferenceType.Liked or PreferenceType.Neutral)
            .Select(p => p.GenreId)
            .ToHashSet();
        var otherUserAuthorPreferences = otherUser.UserAuthorPreferences
            .Where(p => p.PreferenceType is  PreferenceType.Liked or PreferenceType.Neutral)
            .Select(p => p.AuthorId)
            .ToHashSet();
        var readBooksJaccard = Utils.CalculateJaccard(currentUserReadBookIds, otherUserReadBookIds);
        var genresJaccard = Utils.CalculateJaccard(currentUserGenrePreferences.Select(p => p.GenreId).ToHashSet(),
            otherUserGenrePreferences);
        var authorsJaccard = Utils.CalculateJaccard(currentUserAuthorPreferences.Select(p => p.AuthorId).ToHashSet(),
            otherUserAuthorPreferences);
        var volumePreferenceCorresponds = currentUser.VolumeSizePreference is not null && otherUser.VolumeSizePreference is not null &&
                                          currentUser.VolumeSizePreference == otherUser.VolumeSizePreference ? 1 : 0;
        var ageCategoryCorresponds = currentUser.AgeCategory is not null && otherUser.AgeCategory is not null && 
                                     currentUser.AgeCategory == otherUser.AgeCategory ? 1 : 0;
        var similarityWeight = SimilarityScores.Scores[BookSimilarityType.ByReadBooks] * readBooksJaccard
                               + SimilarityScores.Scores[BookSimilarityType.ByAgeRestriction] * ageCategoryCorresponds
                               + SimilarityScores.Scores[BookSimilarityType.ByAuthor] * authorsJaccard
                               + SimilarityScores.Scores[BookSimilarityType.ByGenre] * genresJaccard
                               + SimilarityScores.Scores[BookSimilarityType.ByVolumeSize] * volumePreferenceCorresponds;
        return Utils.NormalizeSimilarity(similarityWeight, SimilarityScores.MaxPossibleScoreForPossiblyLikedBooksHandler);
    }
    
    private async Task<List<Book>> GetYouMayLikeBooksAsync(List<User> similarUsers, CancellationToken cancellationToken)
    {
        const string readBooksCollectionTitle = StaticBookCollectionsData.Read;
        const string favoriteBooksCollectionTitle = StaticBookCollectionsData.Favorite;
        const string readingBooksCollectionTitle = StaticBookCollectionsData.Reading;
        var bookIds = new HashSet<Guid>();
        var userIds = similarUsers.Select(u => u.Id).ToHashSet();
        var possibleGoodBookRatings = await booklyDbContext.Ratings
            .Where(r => userIds.Contains(r.UserId) && r.Value >= 4)
            .Select(r => r.EntityId)
            .ToHashSetAsync(cancellationToken);
        bookIds.UnionWith(possibleGoodBookRatings);
        var personalCollectionsBooks = await booklyDbContext.BookCollections
            .Where(bc => bc.IsStatic && userIds.Contains(bc.UserId) &&
                         (bc.Title == readBooksCollectionTitle || bc.Title == favoriteBooksCollectionTitle || bc.Title == readingBooksCollectionTitle))
            .SelectMany(bc => bc.Books.Select(b => b.Id))
            .ToHashSetAsync(cancellationToken);
        bookIds.UnionWith(personalCollectionsBooks);
        var books = await booklyDbContext.Books
            .Include(b => b.Genres)
            .Include(b => b.Authors)
            .Where(b => bookIds.Contains(b.Id)).ToListAsync(cancellationToken);
        return books.OrderBy(_ => Guid.NewGuid()).ToList();
    }

    private async Task<List<Book>> GetBestBooksOfGenresAsync(List<UserGenrePreference> genrePreferences, CancellationToken cancellationToken)
    {
        var weightsTable = new Dictionary<Guid, double>();
        foreach (var genrePreference in genrePreferences)
            weightsTable[genrePreference.GenreId] = genrePreference.Weight;
        var genrePreferencesIds = weightsTable.Select(g => g.Key).ToHashSet();
        var genreIds = await booklyDbContext.Genres
            .Where(g => genrePreferencesIds.Contains(g.Id) || genrePreferences.Count < TrustedGenrePreferencesCount)
            .Select(g => g.Id)
            .ToListAsync(cancellationToken);
        var averageBooksRating = await mediator.Send(new CalculateAverageRatingQuery<Book>(booklyDbContext.Books), cancellationToken);
        var queryableBooks = booklyDbContext.Books.AsNoTracking()
            .Include(b => b.Genres)
            .Include(b => b.Authors);
        var bestBooks = new List<Book>();
        foreach (var genreId in genreIds)
        {
            var hasWeight = weightsTable.TryGetValue(genreId, out var genreWeight);
            var takeBooksCount = hasWeight ? (int)(MaxBooksFromGenre * (0.7 + genreWeight / 2)) : MaxBooksFromGenre;
            var bestBooksOfGenre = await queryableBooks
                .Where(b => b.Genres.Any(g => g.Id == genreId))
                .OrderItemsByDescendingWeightedRatings(averageBooksRating)
                .Take(takeBooksCount)
                .ToListAsync(cancellationToken);
            bestBooks.AddRange(bestBooksOfGenre);
        }
        return bestBooks.DistinctBy(b => b.Id)
            .OrderBy(_ => Guid.NewGuid())
            .ToList();
    }
}

public record GetPossiblyLikedBooksQuery(BookSimpleSearchSettingsDto BookSimpleSearchSettingsDto, Guid? UserId) 
    : IRequest<Result<List<GetShortBookDto>>>;