using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core.Dto.Preferences;
using Core.Dto.UserGenrePreference;
using Core.Enums;
using Core.Payloads;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace Bookly.Application.Handlers.Preferences;

public class UpdateUserPreferencesHandler(BooklyDbContext booklyDbContext, ILogger logger) : IRequestHandler<UpdateUserPreferencesCommand, Unit>
{
    public async Task<Unit> Handle(UpdateUserPreferencesCommand request, CancellationToken cancellationToken)
    {
        if (!(await booklyDbContext.Users.AnyAsync(u => u.Id ==  request.PreferencePayloadDto.UserId, cancellationToken)))
        {
            logger.Error("Не удается обновить предпочтения у пользователя {userId}: несуществующий пользователь",  request.PreferencePayloadDto.UserId);
            return Unit.Value;
        }
        var book = await booklyDbContext.Books.FirstOrDefaultAsync(b => b.Id == request.PreferencePayloadDto.BookId,
            cancellationToken);
        if (book is null)
        {
            logger.Error("Не удается обновить предпочтения у пользователя {userId}: несуществующая книга", request.PreferencePayloadDto.UserId);
            return Unit.Value;
        }
        await booklyDbContext.Entry(book).Collection(b => b.Authors).LoadAsync(cancellationToken);
        var bookAuthors = book.Authors.Select(a => a.Id).ToHashSet();
        await booklyDbContext.Entry(book).Collection(b => b.Genres).LoadAsync(cancellationToken);
        var bookGenres = book.Genres.Select(g => g.Id).ToHashSet();
        var userAuthorPreferences = await booklyDbContext.UserAuthorPreferences
            .Where(p => p.UserId == request.PreferencePayloadDto.UserId)
            .ToListAsync(cancellationToken);
        var userGenrePreferences = await booklyDbContext.UserGenrePreferences
            .Where(p => p.UserId == request.PreferencePayloadDto.UserId)
            .ToListAsync(cancellationToken);
        var preferenceDelta = DispatchPreferenceActionPayload(request.PreferencePayloadDto.PrerefenceActionPayload);
        if (preferenceDelta != 0)
            await AddOrUpdatePreferencesAsync(bookAuthors, bookGenres, userAuthorPreferences, userGenrePreferences, preferenceDelta,
                request.PreferencePayloadDto.UserId);
        return Unit.Value;
    }

    private async Task AddOrUpdatePreferencesAsync(HashSet<Guid> bookAuthors, HashSet<Guid> bookGenres,
        List<UserAuthorPreference> userAuthorPreferences,
        List<UserGenrePreference> userGenrePreferences, double preferenceDelta, Guid userId)
    {
        var newAuthorPreferences = new List<UserAuthorPreference>();
        var newGenrePreferences = new List<UserGenrePreference>();
        foreach (var userAuthorPreference in userAuthorPreferences)
        {
            if (!bookAuthors.Contains(userAuthorPreference.AuthorId)) continue;
            userAuthorPreference.SetWeight(CalculateNewWeight(userAuthorPreference.Weight, preferenceDelta));
            bookAuthors.Remove(userAuthorPreference.AuthorId);
        }
        foreach (var userGenrePreference in userGenrePreferences)
        {
            if (!bookGenres.Contains(userGenrePreference.GenreId)) continue;
            userGenrePreference.SetWeight(CalculateNewWeight(userGenrePreference.Weight, preferenceDelta));
            bookGenres.Remove(userGenrePreference.GenreId);
        }
        foreach (var genre in bookGenres)
            newGenrePreferences.Add(UserGenrePreference.Create(new UserPreferenceDto(userId, genre, PreferenceType.Neutral,
                CalculateNewWeight(0, preferenceDelta))));
        foreach (var author in bookAuthors)
            newAuthorPreferences.Add(UserAuthorPreference.Create(new UserPreferenceDto(userId, author, PreferenceType.Neutral,
                CalculateNewWeight(0, preferenceDelta))));
        await booklyDbContext.UserAuthorPreferences.AddRangeAsync(newAuthorPreferences);
        await booklyDbContext.UserGenrePreferences.AddRangeAsync(newGenrePreferences);
    }

    private static double CalculateNewWeight(double oldWeight, double preferenceDelta) =>
        0.83 * oldWeight + 0.17 * (oldWeight + preferenceDelta);

    private static double DispatchPreferenceActionPayload(IPrerefenceActionPayload prerefenceActionPayload)
    {
        return prerefenceActionPayload switch
        {
            RemovedFromFavouritesPreferenceActionPayload p => -0.3,
            AddedToFavouritesPreferenceActionPayload p => 0.3,
            RatedPreferenceActionPayload p => (p.Rating - 3) / (double)5,
            ReadBookPreferenceActionPayload p => 0.2,
            StartedToReadBookPreferenceActionPayload p => 0.15,
            WantToReadPreferenceActionPayload p => 0.1,
            RespondedToRecommendationPreferenceActionPayload p => p.RecommendationStatus ==
                                                                  RecommendationStatus.Relevant
                ? 0.3
                : -0.3,
            _ => 0
        };
    }
}

public record UpdateUserPreferencesCommand(PreferencePayloadDto PreferencePayloadDto) : IRequest<Unit>;