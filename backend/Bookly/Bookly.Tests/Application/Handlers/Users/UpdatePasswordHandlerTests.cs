using Bookly.Application.Handlers.Passwords;
using Bookly.Application.Handlers.Users;
using Bookly.Application.Services.Passwords;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core;
using Core.Dto.User;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Bookly.Tests.Application.Handlers.Users;

[TestFixture]
public class UpdateUserPasswordHandlerTests
{
    private BooklyDbContext _db = null!;
    private IMediator _mediator = null!;
    private IPasswordHasher _passwordHasher = null!;

    [SetUp]
    public void Setup()
    {
        _db = DatabaseUtils.CreateDbContext();
        _mediator = Substitute.For<IMediator>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
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
        var dto = new UpdatePasswordDto("oldpwd", "newpwd");
        var handler = new UpdateUserPasswordHandler(_mediator, _db, _passwordHasher);
        var command = new UpdateUserPasswordCommand(Guid.NewGuid(), dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Does.Contain("несуществующего пользователя"));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenOldPasswordIncorrect()
    {
        // Arrange
        var user = User.Create(new CreateUserDto("login", "mail@mail.com", "hash")).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        _passwordHasher.Verify("oldpwd", "hash").Returns(false);

        var dto = new UpdatePasswordDto("oldpwd", "newpwd");
        var handler = new UpdateUserPasswordHandler(_mediator, _db, _passwordHasher);
        var command = new UpdateUserPasswordCommand(user.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Does.Contain("неверный"));
        await _mediator.DidNotReceiveWithAnyArgs().Send(default!, default);
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenHashingFails()
    {
        // Arrange
        var user = User.Create(new CreateUserDto("login", "mail@mail.com", "old_hash")).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        _passwordHasher.Verify("oldpwd", "old_hash").Returns(true);
        _mediator.Send(Arg.Any<HashPasswordCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<string>.Failure("ошибка хеша"));

        var dto = new UpdatePasswordDto("oldpwd", "newpwd");
        var handler = new UpdateUserPasswordHandler(_mediator, _db, _passwordHasher);
        var command = new UpdateUserPasswordCommand(user.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Does.Contain("ошибка хеша"));
        var unchanged = await _db.Users.FirstAsync();
        Assert.That(unchanged.PasswordHash, Is.EqualTo("old_hash"));
    }

    [Test]
    public async Task Handle_UpdatesPassword_WhenAllValid()
    {
        // Arrange
        var user = User.Create(new CreateUserDto("login", "mail@mail.com", "old_hash")).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        _passwordHasher.Verify("oldpwd", "old_hash").Returns(true);
        _mediator.Send(Arg.Any<HashPasswordCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<string>.Success("new_hash"));

        var dto = new UpdatePasswordDto("oldpwd", "newpwd");
        var handler = new UpdateUserPasswordHandler(_mediator, _db, _passwordHasher);
        var command = new UpdateUserPasswordCommand(user.Id, dto);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);

        var updatedUser = await _db.Users.FirstAsync();
        Assert.That(updatedUser.PasswordHash, Is.EqualTo("new_hash"));

        await _mediator.Received(1).Send(
            Arg.Is<HashPasswordCommand>(cmd => cmd.Password == "newpwd"),
            Arg.Any<CancellationToken>());
    }
}