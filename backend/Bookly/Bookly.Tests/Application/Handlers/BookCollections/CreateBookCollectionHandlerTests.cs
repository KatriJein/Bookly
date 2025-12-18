using Bookly.Application.Handlers.BookCollections;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.BookCollection;
using Core.Dto.User;

namespace Bookly.Tests.Application.Handlers.BookCollections;

[TestFixture]
public class CreateBookCollectionHandlerTests
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
    public async Task Handle_CreatesCollection_ForExistingUser()
    {
        var userDto = new CreateUserDto("TestUser", "test@mail.com", "hash123");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var dto = new CreateBookCollectionDto("Моя коллекция", true, user.Id);
        var command = new CreateBookCollectionCommand(dto);
        var handler = new CreateBookCollectionHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(result.Value.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(_db.BookCollections.Count(), Is.EqualTo(1));

        var collection = _db.BookCollections.First();
        Assert.That(collection.Title, Is.EqualTo("Моя коллекция"));
        Assert.That(collection.IsPublic);
        Assert.That(!collection.IsStatic);
        Assert.That(collection.UserId, Is.EqualTo(user.Id));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenUserDoesNotExist()
    {
        var dto = new CreateBookCollectionDto("Пустая коллекция", true, Guid.NewGuid());
        var command = new CreateBookCollectionCommand(dto);
        var handler = new CreateBookCollectionHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Несуществующий пользователь"));
        Assert.That(_db.BookCollections.Any(), Is.False);
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenTitleInvalid()
    {
        var userDto = new CreateUserDto("BadTitleUser", "helloUser@mail.ru", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var dto = new CreateBookCollectionDto("", true, user.Id);
        var command = new CreateBookCollectionCommand(dto);
        var handler = new CreateBookCollectionHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(_db.BookCollections.Any(), Is.False);
    }
}