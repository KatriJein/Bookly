using Bookly.Application.Handlers.Authors;
using Bookly.Application.Handlers.Genres;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core;
using Core.Dto.Author;
using Core.Dto.Genre;
using Core.Dto.Survey;
using Core.Dto.UserGenrePreference;
using Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Survey;

public class SaveEntrySurveyResponsesHandler(IMediator mediator, BooklyDbContext booklyDbContext) : IRequestHandler<SaveEntrySurveyResponsesCommand, Result>
{
    public async Task<Result> Handle(SaveEntrySurveyResponsesCommand request, CancellationToken cancellationToken)
    {
        var user = await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.EntrySurveyDataDto.UserId, cancellationToken);
        if (user is null) return Result.Failure("Пользователь не существует");
        if (user.TookEntrySurvey) return Result.Failure("Входная анкета интересов уже заполнена");
        var favoriteGenresHashset = request.EntrySurveyDataDto.FavoriteGenres.ToHashSet();
        var hatedGenresHashset = request.EntrySurveyDataDto.HatedGenres.ToHashSet();
        var favoriteAuthorsHashset = request.EntrySurveyDataDto.FavoriteAuthors.ToHashSet();
        var hatedAuthorsHashset = request.EntrySurveyDataDto.HatedAuthors.ToHashSet();
        var combinedGenresHashset = favoriteGenresHashset.Union(hatedGenresHashset).ToHashSet();
        var combinedAuthorsHashset =  favoriteAuthorsHashset.Union(hatedAuthorsHashset).ToHashSet();
        if (DublicatesPresented(combinedGenresHashset, favoriteGenresHashset, hatedGenresHashset) ||
            DublicatesPresented(combinedAuthorsHashset, favoriteAuthorsHashset, hatedAuthorsHashset))
            return Result.Failure("Один и тот же жанр / автор выбран в любимых и нелюбимых или присутствуют дубликаты");
        await booklyDbContext.Entry(user).Collection(u => u.UserGenrePreferences).LoadAsync(cancellationToken);
        await booklyDbContext.Entry(user).Collection(u => u.UserAuthorPreferences).LoadAsync(cancellationToken);
        var formedPreferences = await FormPreferences(request.EntrySurveyDataDto.UserId, combinedGenresHashset, combinedAuthorsHashset,
             favoriteGenresHashset, hatedGenresHashset, favoriteAuthorsHashset, hatedAuthorsHashset, cancellationToken);
        var newPreferences = PickNewPreferences(formedPreferences, user.UserGenrePreferences, user.UserAuthorPreferences);
        user.AddGenrePreferences(newPreferences.GenrePreferences);
        user.AddAuthorPreferences(newPreferences.AuthorPreferences);
        user.SetAgeCategory(request.EntrySurveyDataDto.AgeCategory);
        user.SetVolumeSizePreference(request.EntrySurveyDataDto.VolumeSizePreference);
        user.SetHatedGenresRestriction(request.EntrySurveyDataDto.HatedGenresToBlacklist);
        user.MarkEntrySurveyTaken();
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static bool DublicatesPresented(HashSet<string> combined, HashSet<string> favorites, HashSet<string> hated) =>
        combined.Count < favorites.Count + hated.Count;

    private async Task<CreatedPreferences> FormPreferences(Guid userId, HashSet<string> combinedGenres, HashSet<string> combinedAuthors,
        HashSet<string> favoriteGenresHashset, HashSet<string> hatedGenresHashset, HashSet<string> favoriteAuthorsHashset,
        HashSet<string> hatedAuthorsHashset, CancellationToken cancellationToken)
    {
        var genreDtos = combinedGenres.Select(genre => new CreateGenreDto(genre, genre));
        var authorDtos = combinedAuthors.Select(author => new CreateAuthorDto(author, author));
        var allRequiredGenres = new List<Genre>();
        var allRequiredAuthors = new  List<Author>();
        foreach (var genreDto in genreDtos)
        {
            var genre = await mediator.Send(new AddNewGenreCommand(genreDto), cancellationToken);
            if (genre.IsSuccess) allRequiredGenres.Add(genre.Value);
        }
        foreach (var authorDto in authorDtos)
        {
            var author = await mediator.Send(new CreateAuthorCommand(authorDto), cancellationToken);
            if (author.IsSuccess) allRequiredAuthors.Add(author.Value);
        }
        var genrePreferences = allRequiredGenres
            .Select(g => EntityToPreferenceDtoWithExcludingFromHashSets(userId, g.Id, g.Name, favoriteGenresHashset, hatedGenresHashset))
            .ToHashSet();
        var authorPreferences = allRequiredAuthors
            .Select(a => EntityToPreferenceDtoWithExcludingFromHashSets(userId, a.Id, a.Name, favoriteAuthorsHashset, hatedAuthorsHashset))
            .ToHashSet();
        return new CreatedPreferences(genrePreferences, authorPreferences);
    }

    private SelectedPreferences PickNewPreferences(CreatedPreferences formedPreferences,
        IReadOnlyCollection<UserGenrePreference> userGenrePreferences,
        IReadOnlyCollection<UserAuthorPreference> userAuthorPreferences)
    {
        var existingUserGenrePreferences = userGenrePreferences.Select(p => new UserPreferenceDto(p.UserId,
            p.GenreId, p.PreferenceType)).ToHashSet();
        var existingUserAuthorPreferences = userAuthorPreferences.Select(p => new UserPreferenceDto(p.UserId,
            p.AuthorId, p.PreferenceType)).ToHashSet();
        var newGenrePreferences = formedPreferences.GenrePreferences
            .Except(existingUserGenrePreferences)
            .Select(UserGenrePreference.Create);
        var newAuthorPreferences = formedPreferences.AuthorPreferences
            .Except(existingUserAuthorPreferences)
            .Select(UserAuthorPreference.Create);
        return new SelectedPreferences(newGenrePreferences, newAuthorPreferences);
    }

    private UserPreferenceDto EntityToPreferenceDtoWithExcludingFromHashSets(Guid userId, Guid entityId, string entityName, HashSet<string> favoritesHashset,
        HashSet<string> hatedHashset)
    {
        var preferenceType = PreferenceType.Liked;
        if (favoritesHashset.Contains(entityName))
            favoritesHashset.Remove(entityName);
        if (hatedHashset.Contains(entityName))
        {
            hatedHashset.Remove(entityName);
            preferenceType = PreferenceType.Disliked;
        }
        return new UserPreferenceDto(userId, entityId, preferenceType);
    }
}

public record SaveEntrySurveyResponsesCommand(EntrySurveyDataDto EntrySurveyDataDto) : IRequest<Result>;

public record CreatedPreferences(HashSet<UserPreferenceDto> GenrePreferences,
    HashSet<UserPreferenceDto> AuthorPreferences);
    
public record SelectedPreferences(IEnumerable<UserGenrePreference> GenrePreferences,
    IEnumerable<UserAuthorPreference> AuthorPreferences);