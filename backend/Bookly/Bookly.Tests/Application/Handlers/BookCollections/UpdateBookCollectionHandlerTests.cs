using Bookly.Application.Handlers.BookCollections;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.BookCollection;
using Core.Dto.User;

namespace Bookly.Tests.Application.Handlers.BookCollections;

[TestFixture]
public class UpdateBookCollectionHandlerTests
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
    public async Task Handle_UpdatesTitleAndIsPublic_WhenUserOwnsCollection()
    {
        var userDto = new CreateUserDto("UpdUser", "u@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var collectionDto = new CreateBookCollectionDto("Старая коллекция", false, user.Id);
        var collection = BookCollection.Create(collectionDto, false).Value;
        _db.BookCollections.Add(collection);
        await _db.SaveChangesAsync();

        var updateDto = new UpdateBookCollectionDto("Новая коллекция", true);
        var command = new UpdateBookCollectionCommand(updateDto, collection.Id, user.Id);
        var handler = new UpdateBookCollectionHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsSuccess);
        var updated = _db.BookCollections.First();
        Assert.That(updated.Title, Is.EqualTo("Новая коллекция"));
        Assert.That(updated.IsPublic, Is.True);
        Assert.That(updated.UpdatedAt, Is.GreaterThan(updated.CreatedAt));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenCollectionNotFoundOrNotOwned()
    {
        var userDto1 = new CreateUserDto("Owner", "o@mail.com", "hash");
        var userDto2 = new CreateUserDto("Other", "x@mail.com", "hash");
        var owner = User.Create(userDto1).Value;
        var stranger = User.Create(userDto2).Value;
        _db.Users.AddRange(owner, stranger);
        await _db.SaveChangesAsync();

        var collectionDto = new CreateBookCollectionDto("Коллекция", true, owner.Id);
        var collection = BookCollection.Create(collectionDto, false).Value;
        _db.BookCollections.Add(collection);
        await _db.SaveChangesAsync();

        var updateDto = new UpdateBookCollectionDto("Изменить", false);
        var command = new UpdateBookCollectionCommand(updateDto, collection.Id, stranger.Id);
        var handler = new UpdateBookCollectionHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Несуществующая коллекция или не принадлежащая пользователю"));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenCollectionIsStatic()
    {
        var userDto = new CreateUserDto("StaticUser", "s@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var collectionDto = new CreateBookCollectionDto("Статичная", true, user.Id);
        var collection = BookCollection.Create(collectionDto, true).Value;
        _db.BookCollections.Add(collection);
        await _db.SaveChangesAsync();

        var updateDto = new UpdateBookCollectionDto("Измененное имя", false);
        var command = new UpdateBookCollectionCommand(updateDto, collection.Id, user.Id);
        var handler = new UpdateBookCollectionHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Нельзя редактировать статичную коллекцию"));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenTitleInvalid()
    {
        var userDto = new CreateUserDto("InvalidTitle", "i@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var collectionDto = new CreateBookCollectionDto("Коллекция", true, user.Id);
        var collection = BookCollection.Create(collectionDto, false).Value;
        _db.BookCollections.Add(collection);
        await _db.SaveChangesAsync();

        var updateDto = new UpdateBookCollectionDto("", null);
        var command = new UpdateBookCollectionCommand(updateDto, collection.Id, user.Id);
        var handler = new UpdateBookCollectionHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(_db.BookCollections.First().Title, Is.EqualTo("Коллекция"));
    }

    [Test]
    public async Task Handle_UpdatesOnlyVisibility_WhenTitleIsNull()
    {
        var userDto = new CreateUserDto("VisUser", "v@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var collectionDto = new CreateBookCollectionDto("Старая коллекция", false, user.Id);
        var collection = BookCollection.Create(collectionDto, false).Value;
        _db.BookCollections.Add(collection);
        await _db.SaveChangesAsync();

        var updateDto = new UpdateBookCollectionDto(null, true);
        var command = new UpdateBookCollectionCommand(updateDto, collection.Id, user.Id);
        var handler = new UpdateBookCollectionHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsSuccess);
        var updated = _db.BookCollections.First();
        Assert.That(updated.Title, Is.EqualTo("Старая коллекция"));
        Assert.That(updated.IsPublic, Is.True);
    }
}