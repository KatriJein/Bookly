using Bookly.Application.Handlers.BookCollections;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.BookCollection;
using Core.Dto.User;

namespace Bookly.Tests.Application.Handlers.BookCollections;

[TestFixture]
public class DeleteBookCollectionHandlerTests
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
    public async Task Handle_DeletesCollection_WhenUserOwnsIt()
    {
        var userDto = new CreateUserDto("User1", "u1@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var collectionDto = new CreateBookCollectionDto("Коллекция", true, user.Id);
        var collection = BookCollection.Create(collectionDto, false).Value;
        _db.BookCollections.Add(collection);
        await _db.SaveChangesAsync();

        var command = new DeleteBookCollectionCommand(collection.Id, user.Id);
        var handler = new DeleteBookCollectionHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(_db.BookCollections.Any(), Is.False);
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenCollectionIsStatic()
    {
        var userDto = new CreateUserDto("UserStatic", "s@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var collectionDto = new CreateBookCollectionDto("Статичная", true, user.Id);
        var collection = BookCollection.Create(collectionDto, true).Value;
        _db.BookCollections.Add(collection);
        await _db.SaveChangesAsync();

        var command = new DeleteBookCollectionCommand(collection.Id, user.Id);
        var handler = new DeleteBookCollectionHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Невозможно удалить статическую коллекцию"));
        Assert.That(_db.BookCollections.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task Handle_DoesNothing_WhenCollectionDoesNotBelongToUser()
    {
        var ownerDto = new CreateUserDto("Owner", "o@mail.com", "hash");
        var strangerDto = new CreateUserDto("Stranger", "s@mail.com", "hash");
        var owner = User.Create(ownerDto).Value;
        var stranger = User.Create(strangerDto).Value;
        _db.Users.AddRange(owner, stranger);
        await _db.SaveChangesAsync();

        var collectionDto = new CreateBookCollectionDto("Чужая коллекция", false, owner.Id);
        var collection = BookCollection.Create(collectionDto, false).Value;
        _db.BookCollections.Add(collection);
        await _db.SaveChangesAsync();

        var command = new DeleteBookCollectionCommand(collection.Id, stranger.Id);
        var handler = new DeleteBookCollectionHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(_db.BookCollections.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task Handle_Succeeds_WhenCollectionNotFound()
    {
        var userDto = new CreateUserDto("User", "u@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var command = new DeleteBookCollectionCommand(Guid.NewGuid(), user.Id);
        var handler = new DeleteBookCollectionHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(_db.BookCollections.Any(), Is.False);
    }
}