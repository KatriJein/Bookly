using Bookly.Application.Handlers.Users;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.User;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Tests.Application.Handlers.Users;

[TestFixture]
public class UpdateUserHandlerTests
{
    private BooklyDbContext _db = null!;

    [SetUp]
    public void Setup()
    {
        _db = DatabaseUtils.CreateDbContext();
    }

    [TearDown]
    public void TearDown()
    {
        _db.Dispose();
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenUserNotFound()
    {
        // Arrange
        var dto = new UpdateUserDto("newlogin", "new@mail.com");
        var handler = new UpdateUserHandler(_db);
        var command = new UpdateUserCommand(Guid.NewGuid(), dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Does.Contain("Id несуществующего пользователя"));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenLoginAlreadyTakenByAnotherUser()
    {
        // Arrange
        var user1 = User.Create(new CreateUserDto("old1", "a@mail.com", "hash")).Value;
        var user2 = User.Create(new CreateUserDto("taken", "b@mail.com", "hash")).Value;
        _db.Users.AddRange(user1, user2);
        await _db.SaveChangesAsync();

        var dto = new UpdateUserDto("taken", null);
        var handler = new UpdateUserHandler(_db);
        var command = new UpdateUserCommand(user1.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Does.Contain("Пользователь с таким логином"));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenEmailAlreadyTakenByAnotherUser()
    {
        // Arrange
        var user1 = User.Create(new CreateUserDto("login1", "my@mail.com", "hash")).Value;
        var user2 = User.Create(new CreateUserDto("login2", "taken@mail.com", "hash")).Value;
        _db.Users.AddRange(user1, user2);
        await _db.SaveChangesAsync();

        var dto = new UpdateUserDto(null, "taken@mail.com");
        var handler = new UpdateUserHandler(_db);
        var command = new UpdateUserCommand(user1.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Does.Contain("Пользователь с таким логином"));
    }

    [Test]
    public async Task Handle_UpdatesLoginAndEmail_WhenValid()
    {
        // Arrange
        var user = User.Create(new CreateUserDto("oldLogin", "old@mail.com", "pwd")).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var dto = new UpdateUserDto("newLogin", "new@mail.com");
        var handler = new UpdateUserHandler(_db);
        var command = new UpdateUserCommand(user.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var saved = await _db.Users.FirstAsync();
        Assert.That(saved.Login.Value, Is.EqualTo("newLogin"));
        Assert.That(saved.Email.Value, Is.EqualTo("new@mail.com"));
    }

    [Test]
    public async Task Handle_PreservesExistingValues_WhenDtoHasNulls()
    {
        // Arrange
        var user = User.Create(new CreateUserDto("keepLogin", "keep@mail.com", "pwd")).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var dto = new UpdateUserDto(null, null);
        var handler = new UpdateUserHandler(_db);
        var command = new UpdateUserCommand(user.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var saved = await _db.Users.FirstAsync();
        Assert.That(saved.Login.Value, Is.EqualTo("keepLogin"));
        Assert.That(saved.Email.Value, Is.EqualTo("keep@mail.com"));
    }
}