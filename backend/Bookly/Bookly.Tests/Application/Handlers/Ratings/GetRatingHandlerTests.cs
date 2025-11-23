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
    public async Task Handle_SetsUserRating_OnlyForEntitiesWithExistingRatings()
    {
        var userDto = new CreateUserDto("User", "u@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);

        var anotherUserDto = new CreateUserDto("Other", "x@mail.com", "hash");
        var anotherUser = User.Create(anotherUserDto).Value;
        _db.Users.Add(anotherUser);

        var book1 = Book.Create(new CreateBookDto("Book1", "d", 0, 0, "ru", "Pub", 2020, 200,
            AgeRestriction.Everyone, null, "k1", Array.Empty<string>(), Array.Empty<string>())).Value;
        var book2 = Book.Create(new CreateBookDto("Book2", "d", 0, 0, "ru", "Pub", 2020, 200,
            AgeRestriction.Everyone, null, "k2", Array.Empty<string>(), Array.Empty<string>())).Value;
        var book3 = Book.Create(new CreateBookDto("Book3", "d", 0, 0, "ru", "Pub", 2021, 250,
            AgeRestriction.Everyone, null, "k3", Array.Empty<string>(), Array.Empty<string>())).Value;

        _db.Books.AddRange(book1, book2, book3);
        await _db.SaveChangesAsync();

        var rating1 = Rating.Create(user.Id, book1.Id, 4).Value;
        var rating2 = Rating.Create(user.Id, book3.Id, 2).Value;
        var foreignRating = Rating.Create(anotherUser.Id, book1.Id, 5).Value;
        _db.Ratings.AddRange(rating1, rating2, foreignRating);
        await _db.SaveChangesAsync();

        var handler = new GetRatingHandler<Book>(_db);
        var query = new GetRatingQuery<Book>(new List<Book> { book1, book2, book3 }, user.Id);

        await handler.Handle(query, CancellationToken.None);

        Assert.That(book1.UserRating, Is.EqualTo(4));
        Assert.That(book2.UserRating, Is.Null, "У пользователя нет оценки для Book2");
        Assert.That(book3.UserRating, Is.EqualTo(2));
    }

    [Test]
    public async Task Handle_DoesNothing_WhenUserIdIsNull()
    {
        var book = Book.Create(new CreateBookDto("Any", "d", 0, 0, "ru", "Pub", 2020, 200,
            AgeRestriction.Everyone, null, "k4", Array.Empty<string>(), Array.Empty<string>())).Value;
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        var handler = new GetRatingHandler<Book>(_db);
        var query = new GetRatingQuery<Book>(new List<Book> { book }, null);

        await handler.Handle(query, CancellationToken.None);

        Assert.That(book.UserRating, Is.Null);
    }

    [Test]
    public async Task Handle_DoesNothing_WhenUserNotFound()
    {
        var book = Book.Create(new CreateBookDto("Book", "d", 0, 0, "ru", "Pub", 2020, 200,
            AgeRestriction.Everyone, null, "k5", Array.Empty<string>(), Array.Empty<string>())).Value;
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        var handler = new GetRatingHandler<Book>(_db);
        var query = new GetRatingQuery<Book>(new List<Book> { book }, Guid.NewGuid());

        await handler.Handle(query, CancellationToken.None);

        Assert.That(book.UserRating, Is.Null);
    }

    [Test]
    public async Task Handle_NoRatingsExist_LeavesAllUserRatingsNull()
    {
        var userDto = new CreateUserDto("UserNoRatings", "z@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);

        var book1 = Book.Create(new CreateBookDto("Book1", "d", 0, 0, "ru", "Pub", 2020, 200,
            AgeRestriction.Everyone, null, "k6", Array.Empty<string>(), Array.Empty<string>())).Value;
        var book2 = Book.Create(new CreateBookDto("Book2", "d", 0, 0, "ru", "Pub", 2020, 200,
            AgeRestriction.Everyone, null, "k7", Array.Empty<string>(), Array.Empty<string>())).Value;
        _db.Books.AddRange(book1, book2);
        await _db.SaveChangesAsync();

        var handler = new GetRatingHandler<Book>(_db);
        var query = new GetRatingQuery<Book>(new List<Book> { book1, book2 }, user.Id);

        await handler.Handle(query, CancellationToken.None);

        Assert.That(book1.UserRating, Is.Null);
        Assert.That(book2.UserRating, Is.Null);
    }
}