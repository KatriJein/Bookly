using Bookly.Application.Handlers.Books;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.Book;
using Core.Dto.BookCollection;
using Core.Dto.User;
using Core.Enums;
using Moq;
using Serilog;

namespace Bookly.Tests.Application.Handlers.Books;

[TestFixture]
public class MarkFavoritesHandlerTests
{
    private BooklyDbContext _db = null!;
    private Mock<ILogger> _loggerMock = null!;

    [SetUp]
    public void Setup()
    {
        _db = DatabaseUtils.CreateDbContext();
        _loggerMock = new Mock<ILogger>();
    }

    [TearDown]
    public void Teardown()
    {
        _db.Dispose();
    }

    [Test]
    public async Task Handle_SetsIsFavoriteTrue_ForBooksInFavorites()
    {
        var userDto = new CreateUserDto("User", "u@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);

        var bookInFavDto = new CreateBookDto("Book1", "desc", 4, 100, "ru", "Pub", 2020, 200, 
            AgeRestriction.Everyone, null, "k1", Array.Empty<string>(), Array.Empty<string>());
        var bookOutDto = new CreateBookDto("Book2", "desc", 4, 100, "ru", "Pub", 2020, 200, 
            AgeRestriction.Everyone, null, "k2", Array.Empty<string>(), Array.Empty<string>());
        var bookInFav = Book.Create(bookInFavDto).Value;
        var bookOut = Book.Create(bookOutDto).Value;
        _db.Books.AddRange(bookInFav, bookOut);

        var favoritesCollection = BookCollection.Create(new CreateBookCollectionDto("Избранное", true, user.Id), true).Value;
        favoritesCollection.AddBookAndUpdateCover(bookInFav);
        _db.BookCollections.Add(favoritesCollection);
        await _db.SaveChangesAsync();

        var handler = new MarkFavoritesHandler(_db, _loggerMock.Object);
        var books = new List<Book> { bookInFav, bookOut };

        await handler.Handle(new MarkFavoritesCommand(books, user.Id), CancellationToken.None);

        Assert.That(bookInFav.IsFavorite, Is.True);
        Assert.That(bookOut.IsFavorite, Is.False);
    }

    [Test]
    public async Task Handle_DoesNothing_WhenUserIdIsNullOrEmpty()
    {
        var bookDto = new CreateBookDto("Book", "d", 3, 100, "ru", "Pub", 2020, 200, 
            AgeRestriction.Everyone, null, "k3", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        var books = new List<Book> { book };

        var handler = new MarkFavoritesHandler(_db, _loggerMock.Object);

        await handler.Handle(new MarkFavoritesCommand(books, null), CancellationToken.None);
        Assert.That(book.IsFavorite, Is.False);

        await handler.Handle(new MarkFavoritesCommand(books, Guid.Empty), CancellationToken.None);
        Assert.That(book.IsFavorite, Is.False);
    }

    [Test]
    public async Task Handle_DoesNothing_WhenFavoritesCollectionNotFound()
    {
        var userDto = new CreateUserDto("NoFavUser", "n@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var bookDto = new CreateBookDto("Book", "desc", 3, 100, "ru", "Pub", 2020, 200, 
            AgeRestriction.Everyone, null, "k4", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;

        var handler = new MarkFavoritesHandler(_db, _loggerMock.Object);
        var books = new List<Book> { book };

        await handler.Handle(new MarkFavoritesCommand(books, user.Id), CancellationToken.None);

        Assert.That(book.IsFavorite, Is.False);
    }

    [Test]
    public async Task Handle_SetsAllFalse_WhenFavoritesWithoutBooks()
    {
        var userDto = new CreateUserDto("EmptyFav", "e@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var bookDto = new CreateBookDto("Book", "d", 3, 100, "ru", "Pub", 2020, 200, 
            AgeRestriction.Everyone, null, "k5", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);

        var favorites = BookCollection.Create(new CreateBookCollectionDto("Избранное", true, user.Id), true).Value;
        _db.BookCollections.Add(favorites);
        await _db.SaveChangesAsync();

        var handler = new MarkFavoritesHandler(_db, _loggerMock.Object);
        var books = new List<Book> { book };

        await handler.Handle(new MarkFavoritesCommand(books, user.Id), CancellationToken.None);

        Assert.That(book.IsFavorite, Is.False);
    }
}