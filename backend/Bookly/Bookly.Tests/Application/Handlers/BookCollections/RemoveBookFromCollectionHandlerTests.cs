using Bookly.Application.Handlers.BookCollections;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.Book;
using Core.Dto.BookCollection;
using Core.Dto.User;
using Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Tests.Application.Handlers.BookCollections;

[TestFixture]
public class RemoveBookFromBookCollectionHandlerTests
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
    public async Task Handle_RemovesBook_WhenUserOwnsCollection()
    {
        var userDto = new CreateUserDto("User", "u@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);

        var bookDto = new CreateBookDto("Book", "desc", 4, 100, "ru", "Pub", 2020, 200, AgeRestriction.Everyone, null, "k1", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);

        var collection = BookCollection.Create(new CreateBookCollectionDto("Коллекция", true, user.Id), false).Value;
        collection.AddBookAndUpdateCover(book);
        _db.BookCollections.Add(collection);

        await _db.SaveChangesAsync();

        var handler = new RemoveBookFromBookCollectionHandler(_db);
        var command = new RemoveBookFromBookCollectionCommand(collection.Id, book.Id, user.Id);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsSuccess);
        var updated = _db.BookCollections.Include(c => c.Books).First();
        Assert.That(updated.Books, Is.Empty);
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenBookDoesNotExist()
    {
        var userDto = new CreateUserDto("User2", "u2@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var collection = BookCollection.Create(new CreateBookCollectionDto("Коллекция", true, user.Id), false).Value;
        _db.BookCollections.Add(collection);
        await _db.SaveChangesAsync();

        var handler = new RemoveBookFromBookCollectionHandler(_db);
        var command = new RemoveBookFromBookCollectionCommand(collection.Id, Guid.NewGuid(), user.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Несуществующая книга"));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenUserDoesNotExist()
    {
        var bookDto = new CreateBookDto("Book", "desc", 4, 100, "ru", "Pub", 2020, 200, AgeRestriction.Everyone, null, "k2", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        var handler = new RemoveBookFromBookCollectionHandler(_db);
        var command = new RemoveBookFromBookCollectionCommand(Guid.NewGuid(), book.Id, Guid.NewGuid());
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Несуществующий пользователь"));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenCollectionNotOwnedByUser()
    {
        var ownerDto = new CreateUserDto("Owner", "o@mail.com", "hash");
        var strangerDto = new CreateUserDto("Stranger", "s@mail.com", "hash");
        var owner = User.Create(ownerDto).Value;
        var stranger = User.Create(strangerDto).Value;
        _db.Users.AddRange(owner, stranger);

        var bookDto = new CreateBookDto("Book", "desc", 4, 100, "ru", "Pub", 2020, 200, AgeRestriction.Everyone, null, "k3", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);

        var collection = BookCollection.Create(new CreateBookCollectionDto("Коллекция", true, owner.Id), false).Value;
        collection.AddBookAndUpdateCover(book);
        _db.BookCollections.Add(collection);

        await _db.SaveChangesAsync();

        var handler = new RemoveBookFromBookCollectionHandler(_db);
        var command = new RemoveBookFromBookCollectionCommand(collection.Id, book.Id, stranger.Id);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Несуществующая или не принадлежащая пользователю коллекция"));
        var unchanged = _db.BookCollections.Include(c => c.Books).First();
        Assert.That(unchanged.Books.Any(b => b.Id == book.Id));
    }
}