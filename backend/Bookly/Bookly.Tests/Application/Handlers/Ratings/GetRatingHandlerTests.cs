using Bookly.Application.Handlers.Ratings;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.Book;
using Core.Dto.User;
using Core.Enums;

namespace Bookly.Tests.Application.Handlers.Ratings;

[TestFixture]
public class GetRatingHandlerTests
{
    private BooklyDbContext _db = null!;

    [SetUp]
    public void Setup()
    {
        _db = DatabaseUtils.CreateDbContext();
    }

    [TearDown]
    public void Teardown()
    {
        _db.Dispose();
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenUserDoesNotExist()
    {
        var handler = new GetRatingHandler<Book>(_db);
        var query = new GetRatingQuery<Book>(
            db => db.Books, Guid.NewGuid(), Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Несуществующий пользователь"));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenEntityDoesNotExist()
    {
        var userDto = new CreateUserDto("User", "u@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var handler = new GetRatingHandler<Book>(_db);
        var query = new GetRatingQuery<Book>(
            db => db.Books, user.Id, Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Несуществующая сущность"));
    }

    [Test]
    public async Task Handle_ReturnsNullRating_WhenNoUserRatingFound()
    {
        var userDto = new CreateUserDto("User", "u@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);

        var bookDto = new CreateBookDto("Book", "desc", 4.5, 100, "ru", "Pub", 2020, 200,
            AgeRestriction.Everyone, null, "k1", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        var handler = new GetRatingHandler<Book>(_db);
        var query = new GetRatingQuery<Book>(
            db => db.Books, user.Id, book.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(result.Value, Is.Null);
    }

    [Test]
    public async Task Handle_ReturnsRating_WhenUserRatingExists()
    {
        var userDto = new CreateUserDto("User", "u@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);

        var bookDto = new CreateBookDto("BookName", "desc", 4.2, 150, "ru", "Pub", 2020, 300,
            AgeRestriction.Everyone, null, "k2", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        var rating = Rating.Create(user.Id, book.Id, 5).Value;
        _db.Ratings.Add(rating);
        await _db.SaveChangesAsync();

        var handler = new GetRatingHandler<Book>(_db);
        var query = new GetRatingQuery<Book>(
            db => db.Books, user.Id, book.Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(result.Value, Is.Not.Null);
        Assert.That(result.Value.Rating, Is.EqualTo(5));
    }
}