using Bookly.Application.Handlers.Authors;
using Bookly.Application.Handlers.Genres;
using Bookly.Application.Handlers.Survey;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core;
using Core.Dto.Author;
using Core.Dto.Genre;
using Core.Dto.Survey;
using Core.Dto.User;
using Core.Dto.UserGenrePreference;
using Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Bookly.Tests.Application.Handlers.Surveys;

[TestFixture]
public class SaveEntrySurveyResponsesHandlerTests
{
    private BooklyDbContext _db = null!;
    private IMediator _mediator = null!;

    [SetUp]
    public void Setup()
    {
        _db = DatabaseUtils.CreateDbContext();
        _mediator = Substitute.For<IMediator>();
    }

    [TearDown]
    public void Teardown() => _db.Dispose();

    private static EntrySurveyDataDto BuildSurvey(Guid userId,
        IEnumerable<string>? favGenres = null,
        IEnumerable<string>? hatedGenres = null,
        IEnumerable<string>? favAuthors = null,
        IEnumerable<string>? hatedAuthors = null)
    {
        return new EntrySurveyDataDto(
            userId,
            favGenres?.ToArray() ?? Array.Empty<string>(),
            hatedGenres?.ToArray() ?? Array.Empty<string>(),
            favAuthors?.ToArray() ?? Array.Empty<string>(),
            hatedAuthors?.ToArray() ?? Array.Empty<string>(),
            VolumeSizePreference.Medium,
            AgeCategory.Teen
        );
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenUserNotFound()
    {
        var handler = new SaveEntrySurveyResponsesHandler(_mediator, _db);
        var cmd = new SaveEntrySurveyResponsesCommand(BuildSurvey(Guid.NewGuid()));

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Does.Contain("не существует"));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenSurveyAlreadyTaken()
    {
        var user = User.Create(new CreateUserDto("login", "mail@mail.com", "pwd")).Value;
        user.MarkEntrySurveyTaken();
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var handler = new SaveEntrySurveyResponsesHandler(_mediator, _db);
        var cmd = new SaveEntrySurveyResponsesCommand(BuildSurvey(user.Id));

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Does.Contain("уже заполнена"));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenDuplicatesInGenres()
    {
        var user = User.Create(new CreateUserDto("login", "mail@mail.com", "pwd")).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var dto = BuildSurvey(user.Id,
            favGenres: new[] { "Фантастика" },
            hatedGenres: new[] { "Фантастика" });

        var handler = new SaveEntrySurveyResponsesHandler(_mediator, _db);
        var result = await handler.Handle(new SaveEntrySurveyResponsesCommand(dto), CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Does.Contain("жанр"));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenDuplicatesInAuthors()
    {
        var user = User.Create(new CreateUserDto("login", "mail@mail.com", "pwd")).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var dto = BuildSurvey(user.Id,
            favAuthors: new[] { "Пушкин" },
            hatedAuthors: new[] { "Пушкин" });

        var handler = new SaveEntrySurveyResponsesHandler(_mediator, _db);
        var result = await handler.Handle(new SaveEntrySurveyResponsesCommand(dto), CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Does.Contain("автор"));
    }

    [Test]
    public async Task Handle_CreatesPreferences_WhenAllValid()
    {
        var user = User.Create(new CreateUserDto("newuser", "mail@mail.com", "hash")).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var genre1 = Genre.Create(new CreateGenreDto("Фантастика", "Фантастика")).Value;
        var genre2 = Genre.Create(new CreateGenreDto("Драма", "Драма")).Value;
        var author1 = Author.Create(new CreateAuthorDto("Пушкин", "А.С. Пушкин")).Value;
        var author2 = Author.Create(new CreateAuthorDto("Толстой", "Л.Н. Толстой")).Value;

        _mediator.Send(Arg.Is<AddNewGenreCommand>(c => c.CreateGenreDto.Name == "Фантастика"), Arg.Any<CancellationToken>())
            .Returns(Result<Genre>.Success(genre1));
        _mediator.Send(Arg.Is<AddNewGenreCommand>(c => c.CreateGenreDto.Name == "Драма"), Arg.Any<CancellationToken>())
            .Returns(Result<Genre>.Success(genre2));
        _mediator.Send(Arg.Is<CreateAuthorCommand>(c => c.CreateAuthorDto.Name == "Пушкин"), Arg.Any<CancellationToken>())
            .Returns(Result<Author>.Success(author1));
        _mediator.Send(Arg.Is<CreateAuthorCommand>(c => c.CreateAuthorDto.Name == "Толстой"), Arg.Any<CancellationToken>())
            .Returns(Result<Author>.Success(author2));

        var dto = BuildSurvey(user.Id,
            favGenres: new[] { "Фантастика" },
            hatedGenres: new[] { "Драма" },
            favAuthors: new[] { "Пушкин" },
            hatedAuthors: new[] { "Толстой" });

        var handler = new SaveEntrySurveyResponsesHandler(_mediator, _db);

        var result = await handler.Handle(new SaveEntrySurveyResponsesCommand(dto), CancellationToken.None);

        Assert.That(result.IsSuccess);

        var reloaded = await _db.Users
            .Include(u => u.UserGenrePreferences)
            .Include(u => u.UserAuthorPreferences)
            .FirstAsync();

        Assert.That(reloaded.TookEntrySurvey, Is.True);
        Assert.That(reloaded.AgeCategory, Is.EqualTo(AgeCategory.Teen));
        Assert.That(reloaded.VolumeSizePreference, Is.EqualTo(VolumeSizePreference.Medium));
        Assert.That(reloaded.UserGenrePreferences.Count, Is.EqualTo(2));
        Assert.That(reloaded.UserAuthorPreferences.Count, Is.EqualTo(2));

        Assert.That(reloaded.UserGenrePreferences.Any(p => p.PreferenceType == PreferenceType.Liked));
        Assert.That(reloaded.UserGenrePreferences.Any(p => p.PreferenceType == PreferenceType.Disliked));
        Assert.That(reloaded.UserAuthorPreferences.Any(p => p.PreferenceType == PreferenceType.Liked));
        Assert.That(reloaded.UserAuthorPreferences.Any(p => p.PreferenceType == PreferenceType.Disliked));
    }

    [Test]
    public async Task Handle_SkipsExistingPreferences()
    {
        var user = User.Create(new CreateUserDto("existing", "mail@mail.com", "hash")).Value;

        // фиксированный Id для совпадения
        var genreId = Guid.NewGuid();

        var existingPref = UserGenrePreference.Create(
            new UserPreferenceDto(user.Id, genreId, PreferenceType.Liked));
        user.AddGenrePreferences(new[] { existingPref });
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var genre = Genre.Create(new CreateGenreDto("Фантастика", "Фантастика")).Value;
        typeof(Genre).GetProperty("Id")?.SetValue(genre, genreId);

        _mediator.Send(Arg.Any<AddNewGenreCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<Genre>.Success(genre));
        _mediator.Send(Arg.Any<CreateAuthorCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<Author>.Failure("автор не важен в этом тесте"));

        var dto = BuildSurvey(user.Id,
            favGenres: new[] { "Фантастика" });

        var handler = new SaveEntrySurveyResponsesHandler(_mediator, _db);

        var result = await handler.Handle(new SaveEntrySurveyResponsesCommand(dto), CancellationToken.None);

        Assert.That(result.IsSuccess);
        var updated = await _db.Users.Include(u => u.UserGenrePreferences).FirstAsync();
        Assert.That(updated.UserGenrePreferences.Count, Is.EqualTo(1)); // совпавший не добавлен
    }

    [Test]
    public async Task Handle_IgnoresFailedGenreOrAuthorCreation()
    {
        var user = User.Create(new CreateUserDto("user", "mail@mail.com", "hash")).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        _mediator.Send(Arg.Any<AddNewGenreCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<Genre>.Failure("ошибка жанра"));
        _mediator.Send(Arg.Any<CreateAuthorCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<Author>.Failure("ошибка автора"));

        var dto = BuildSurvey(user.Id,
            favGenres: new[] { "Жанр1" },
            hatedGenres: new[] { "Жанр2" },
            favAuthors: new[] { "Автор1" },
            hatedAuthors: new[] { "Автор2" });

        var handler = new SaveEntrySurveyResponsesHandler(_mediator, _db);

        var result = await handler.Handle(new SaveEntrySurveyResponsesCommand(dto), CancellationToken.None);

        Assert.That(result.IsSuccess);
        var loaded = await _db.Users.Include(u => u.UserGenrePreferences)
                                    .Include(u => u.UserAuthorPreferences)
                                    .FirstAsync();
        Assert.That(loaded.UserGenrePreferences, Is.Empty);
        Assert.That(loaded.UserAuthorPreferences, Is.Empty);
    }

    [Test]
    public async Task Handle_CreatesPreferences_UsingExistingGenresAndAuthors()
    {
        var user = User.Create(new CreateUserDto("userG", "mail@mail.com", "pwd")).Value;
        _db.Users.Add(user);

        var existingGenre = Genre.Create(new CreateGenreDto("Сказка", "Сказка")).Value;
        var existingAuthor = Author.Create(new CreateAuthorDto("Гоголь", "Н.В. Гоголь")).Value;
        _db.Genres.Add(existingGenre);
        _db.Authors.Add(existingAuthor);
        await _db.SaveChangesAsync();

        _mediator.Send(Arg.Is<AddNewGenreCommand>(g => g.CreateGenreDto.Name == "Сказка"), Arg.Any<CancellationToken>())
            .Returns(Result<Genre>.Success(existingGenre));
        _mediator.Send(Arg.Is<CreateAuthorCommand>(a => a.CreateAuthorDto.Name == "Гоголь"), Arg.Any<CancellationToken>())
            .Returns(Result<Author>.Success(existingAuthor));

        var dto = BuildSurvey(user.Id,
            favGenres: new[] { "Сказка" },
            favAuthors: new[] { "Гоголь" });

        var handler = new SaveEntrySurveyResponsesHandler(_mediator, _db);

        var result = await handler.Handle(new SaveEntrySurveyResponsesCommand(dto), CancellationToken.None);

        Assert.That(result.IsSuccess);
        var reloaded = await _db.Users
            .Include(u => u.UserGenrePreferences)
            .Include(u => u.UserAuthorPreferences)
            .FirstAsync();

        Assert.That(reloaded.UserGenrePreferences.Count, Is.EqualTo(1));
        Assert.That(reloaded.UserAuthorPreferences.Count, Is.EqualTo(1));

        Assert.That(reloaded.UserGenrePreferences.First().GenreId, Is.EqualTo(existingGenre.Id));
        Assert.That(reloaded.UserAuthorPreferences.First().AuthorId, Is.EqualTo(existingAuthor.Id));
    }
}