using Bookly.Application.Handlers.Rateable;
using Bookly.Application.Mappers;
using Bookly.Domain.Models;
using Bookly.Extensions;
using Bookly.Infrastructure;
using Core;
using Core.Dto.Book;
using Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Books;

public class GetPossiblyLikedBooksHandler(IMediator mediator, BooklyDbContext booklyDbContext) : IRequestHandler<GetPossiblyLikedBooksQuery, Result<List<GetShortBookDto>>>
{
    private const int MaxSimilarUsers = 20;
    private const double MinRequiredSimilarity = 0.55;
    private const int TrustedGenrePreferencesCount = 3;
    private const int MaxBooksFromGenre = 10;
    
    public async Task<Result<List<GetShortBookDto>>> Handle(GetPossiblyLikedBooksQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId is null) return Result<List<GetShortBookDto>>.Failure("Раздел доступен только авторизованным пользователям");
        var currentUser = await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (currentUser is null) return Result<List<GetShortBookDto>>.Failure("Пользователь не авторизован или не существует");
        await booklyDbContext.Entry(currentUser).Collection(u => u.UserGenrePreferences).LoadAsync(cancellationToken);
        await booklyDbContext.Entry(currentUser).Collection(u => u.UserAuthorPreferences).LoadAsync(cancellationToken);
        var preferredGenres = currentUser.UserGenrePreferences
            .Where(p => p.PreferenceType is PreferenceType.Liked or PreferenceType.Neutral)
            .ToList();
        var preferredAuthors = currentUser.UserAuthorPreferences
            .Where(p => p.PreferenceType is  PreferenceType.Liked or PreferenceType.Neutral)
            .ToList();
        var similarUsers = await FindSimilarUsersAsync(currentUser, preferredGenres, preferredAuthors);
        var books = similarUsers.Count > 0
            ? await GetYouMayLikeBooksAsync(currentUser, similarUsers, cancellationToken)
            : await GetBestBooksOfGenresAsync(preferredGenres, cancellationToken);
        var preparedBooks = await mediator.Send(new ExcludeIrrelevantBooksAndEnrichRelevantWithDataCommand(books,
            request.UserId, request.BookSimpleSearchSettingsDto), cancellationToken);
        var booksDto = preparedBooks.Select(BookMapper.MapBookToShortBookDto).ToList();
        return Result<List<GetShortBookDto>>.Success(booksDto);
    }
    
    private async Task<List<User>> FindSimilarUsersAsync(User currentUser, List<UserGenrePreference> genrePreferences,
        List<UserAuthorPreference> authorPreferences)
    {
        return [];
    }
    
    private async Task<List<Book>> GetYouMayLikeBooksAsync(User currentUser, List<User> similarUsers, CancellationToken cancellationToken)
    {
        return [];
    }

    private async Task<List<Book>> GetBestBooksOfGenresAsync(List<UserGenrePreference> genrePreferences, CancellationToken cancellationToken)
    {
        var weightsTable = genrePreferences.ToDictionary(gp => gp.GenreId, gp => gp.Weight);
        var genrePreferencesIds = weightsTable.Select(g => g.Key).ToHashSet();
        var genreIds = await booklyDbContext.Genres
            .Where(g => genrePreferencesIds.Contains(g.Id) || genrePreferences.Count < TrustedGenrePreferencesCount)
            .Select(g => g.Id)
            .ToListAsync(cancellationToken);
        var averageBooksRating = await mediator.Send(new CalculateAverageRatingQuery<Book>(booklyDbContext.Books), cancellationToken);
        var queryableBooks = booklyDbContext.Books.AsNoTracking().Include(b => b.Genres);
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