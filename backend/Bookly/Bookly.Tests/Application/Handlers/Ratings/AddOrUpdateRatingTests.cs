using Bookly.Application.Handlers.Ratings;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.Book;
using Core.Dto.User;
using Core.Enums;

namespace Bookly.Tests.Application.Handlers.Ratings;

[TestFixture]
public class AddOrUpdateRatingHandlerTests
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
        var handler = new AddOrUpdateRatingHandler<Book>(_db);
        var query = new AddOrUpdateRatingCommand<Book>(
            db => db.Books, Guid.NewGuid(), Guid.NewGuid(), 5);

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

        var handler = new AddOrUpdateRatingHandler<Book>(_db);
        var cmd = new AddOrUpdateRatingCommand<Book>(
            db => db.Books, Guid.NewGuid(), user.Id, 5);

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Несуществующая сущность"));
    }

    [Test]
    public async Task Handle_CreatesNewRating_WhenRatingDoesNotExist()
    {
        var userDto = new CreateUserDto("User", "u@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);

        var bookDto = new CreateBookDto("Book", "desc", 0, 0, "ru", "Pub", 2020, 200,
            AgeRestriction.Everyone, null, "k1", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        var handler = new AddOrUpdateRatingHandler<Book>(_db);
        var cmd = new AddOrUpdateRatingCommand<Book>(
            db => db.Books, book.Id, user.Id, 5);

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.That(result.IsSuccess);
        var createdRating = _db.Ratings.FirstOrDefault(r => r.UserId == user.Id && r.EntityId == book.Id);
        Assert.That(createdRating, Is.Not.Null);
        Assert.That(createdRating!.Value, Is.EqualTo(5));

        var updatedBook = _db.Books.First(b => b.Id == book.Id);
        Assert.That(updatedBook.RatingsCount, Is.EqualTo(1));
        Assert.That(updatedBook.Rating, Is.EqualTo(5));
    }

    [Test]
    public async Task Handle_UpdatesRating_WhenRatingExists()
    {
        var userDto = new CreateUserDto("User", "u2@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);

        var bookDto = new CreateBookDto("Book", "desc", 0, 0, "ru", "Pub", 2020, 200,
            AgeRestriction.Everyone, null, "k2", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        var rating = Rating.Create(user.Id, book.Id, 3).Value;
        _db.Ratings.Add(rating);
        book.AddNewRating(3);
        await _db.SaveChangesAsync();

        var handler = new AddOrUpdateRatingHandler<Book>(_db);
        var cmd = new AddOrUpdateRatingCommand<Book>(
            db => db.Books, book.Id, user.Id, 5);

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.That(result.IsSuccess);

        var persistedRating = _db.Ratings.First(r => r.EntityId == book.Id && r.UserId == user.Id);
        Assert.That(persistedRating.Value, Is.EqualTo(5));

        var updatedBook = _db.Books.First(b => b.Id == book.Id);
        Assert.That(updatedBook.RatingsCount, Is.EqualTo(1));
        Assert.That(updatedBook.Rating, Is.EqualTo(5));
    }

    [Test]
    public async Task Handle_UpdatesAverageRating_WhenAdditionalRatingsExist()
    {
        var userDto1 = new CreateUserDto("User1", "u1@mail.com", "hash");
        var userDto2 = new CreateUserDto("User2", "u2@mail.com", "hash");
        var user1 = User.Create(userDto1).Value;
        var user2 = User.Create(userDto2).Value;
        _db.Users.AddRange(user1, user2);

        var bookDto = new CreateBookDto("Book", "desc", 0, 0, "ru", "Pub", 2020, 200,
            AgeRestriction.Everyone, null, "k3", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        var rating1 = Rating.Create(user1.Id, book.Id, 4).Value;
        var rating2 = Rating.Create(user2.Id, book.Id, 5).Value;
        _db.Ratings.AddRange(rating1, rating2);

        book.AddNewRating(4);
        book.AddNewRating(5);
        await _db.SaveChangesAsync();

        var handler = new AddOrUpdateRatingHandler<Book>(_db);
        var cmd = new AddOrUpdateRatingCommand<Book>(
            db => db.Books, book.Id, user1.Id, 2);

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.That(result.IsSuccess);

        var ratings = _db.Ratings.Where(r => r.EntityId == book.Id).ToList();
        Assert.That(ratings.Count, Is.EqualTo(2));

        var updatedBook = _db.Books.First(b => b.Id == book.Id);
        Assert.That(updatedBook.RatingsCount, Is.EqualTo(2));
        Assert.That(Math.Round(updatedBook.Rating, 2), Is.EqualTo(3.5));
    }
}