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
public class AddBookToBookCollectionsHandlerTests
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
    public async Task Handle_AddsBookToSpecifiedCollections_WhenUserAndBookExist()
    {
        var userDto = new CreateUserDto("User", "u@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var bookDto = new CreateBookDto("Book", "desc", 4.5, 100, "ru", "Publisher", 2020, 200, AgeRestriction.Everyone, null, "k1", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        var c1 = BookCollection.Create(new CreateBookCollectionDto("Коллекция1", true, user.Id), false).Value;
        var c2 = BookCollection.Create(new CreateBookCollectionDto("Коллекция2", true, user.Id), false).Value;
        _db.BookCollections.AddRange(c1, c2);
        await _db.SaveChangesAsync();

        var command = new AddBookToBookCollectionsCommand(new List<Guid> { c1.Id, c2.Id }, book.Id, user.Id);
        var handler = new AddBookToBookCollectionsHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsSuccess);
        var col1 = _db.BookCollections.Include(b => b.Books).First(c => c.Id == c1.Id);
        var col2 = _db.BookCollections.Include(b => b.Books).First(c => c.Id == c2.Id);
        Assert.That(col1.Books.Any(b => b.Id == book.Id));
        Assert.That(col2.Books.Any(b => b.Id == book.Id));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenBookDoesNotExist()
    {
        var userDto = new CreateUserDto("User2", "u2@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var c = BookCollection.Create(new CreateBookCollectionDto("Коллекция", true, user.Id), false).Value;
        _db.BookCollections.Add(c);
        await _db.SaveChangesAsync();

        var command = new AddBookToBookCollectionsCommand(new List<Guid> { c.Id }, Guid.NewGuid(), user.Id);
        var handler = new AddBookToBookCollectionsHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Несуществующая книга"));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenUserDoesNotExist()
    {
        var bookDto = new CreateBookDto("Book", "desc", 4.2, 300, "ru", "Publisher", 2020, 200, AgeRestriction.Everyone, null, "k2", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        var command = new AddBookToBookCollectionsCommand(new List<Guid> { Guid.NewGuid() }, book.Id, Guid.NewGuid());
        var handler = new AddBookToBookCollectionsHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Несуществующий пользователь"));
    }

    [Test]
    public async Task Handle_DoesNotAddBook_ToCollectionsOfAnotherUser()
    {
        var userDto1 = new CreateUserDto("Owner", "o@mail.com", "hash");
        var userDto2 = new CreateUserDto("Other", "x@mail.com", "hash");
        var owner = User.Create(userDto1).Value;
        var other = User.Create(userDto2).Value;
        _db.Users.AddRange(owner, other);
        await _db.SaveChangesAsync();

        var bookDto = new CreateBookDto("SharedBook", "desc", 4.5, 150, "ru", "Pub", 2020, 300, AgeRestriction.Everyone, null, "k3", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);

        var ownerCollection = BookCollection.Create(new CreateBookCollectionDto("collection1", true, owner.Id), false).Value;
        _db.BookCollections.Add(ownerCollection);
        await _db.SaveChangesAsync();

        var command = new AddBookToBookCollectionsCommand(new List<Guid> { ownerCollection.Id }, book.Id, other.Id);
        var handler = new AddBookToBookCollectionsHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsSuccess);
        var collection = _db.BookCollections.Include(c => c.Books).First();
        Assert.That(collection.Books, Is.Empty);
    }
}