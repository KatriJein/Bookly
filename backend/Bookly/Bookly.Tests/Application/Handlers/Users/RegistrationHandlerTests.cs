using Bookly.Application.Handlers.Auth;
using Bookly.Application.Handlers.Passwords;
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
public class RegistrationHandlerTests
{
    private BooklyDbContext _db = null!;
    private IMediator _mediator = null!;

    [SetUp]
    public void Setup()
    {
        _db = DatabaseUtils.CreateDbContext();
        _mediator = Substitute.For<IMediator>();
    }

    [TearDown]
    public void Teardown()
    {
        _db.Dispose();
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenUserAlreadyExists_ByLogin()
    {
        // Arrange
        var existing = User.Create(new CreateUserDto("existing_user", "mail@mail.com", "hash")).Value;
        _db.Users.Add(existing);
        await _db.SaveChangesAsync();

        var dto = new RegistrationRequestDto("existing_user", "another@mail.com", "pwd123!");
        var handler = new RegistrationHandler(_mediator, _db);

        // Act
        var result = await handler.Handle(new RegistrationCommand(dto), CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Does.Contain("уже используются"));
        Assert.That(await _db.Users.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenUserAlreadyExists_ByEmail()
    {
        // Arrange
        var existing = User.Create(new CreateUserDto("login1", "used@mail.com", "hash")).Value;
        _db.Users.Add(existing);
        await _db.SaveChangesAsync();

        var dto = new RegistrationRequestDto("new_login", "USED@mail.com", "pwd123!");
        var handler = new RegistrationHandler(_mediator, _db);

        // Act
        var result = await handler.Handle(new RegistrationCommand(dto), CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(await _db.Users.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenHashPasswordFails()
    {
        // Arrange
        var dto = new RegistrationRequestDto("new_user", "mail@mail.com", "badpwd");
        _mediator.Send(Arg.Any<HashPasswordCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<string>.Failure("ошибка хеша"));

        var handler = new RegistrationHandler(_mediator, _db);

        // Act
        var result = await handler.Handle(new RegistrationCommand(dto), CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Does.Contain("ошибка хеша"));
        Assert.That(await _db.Users.CountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenUserCreationFails()
    {
        // Arrange
        var dto = new RegistrationRequestDto("hellothere", "valid", "pwd123!");
        _mediator.Send(Arg.Any<HashPasswordCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<string>.Success("hash123"));

        var handler = new RegistrationHandler(_mediator, _db);

        // Act
        var result = await handler.Handle(new RegistrationCommand(dto), CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(await _db.Users.CountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task Handle_CreatesUser_WhenAllValid()
    {
        // Arrange
        var dto = new RegistrationRequestDto("new_user", "valid@mail.com", "GoodPwd!");
        _mediator.Send(Arg.Any<HashPasswordCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<string>.Success("hashed_pwd"));

        var handler = new RegistrationHandler(_mediator, _db);

        // Act
        var result = await handler.Handle(new RegistrationCommand(dto), CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(await _db.Users.CountAsync(), Is.EqualTo(1));

        var saved = await _db.Users.FirstAsync();
        Assert.That(saved.Login.Value, Is.EqualTo("new_user"));
        Assert.That(saved.Email.Value, Is.EqualTo("valid@mail.com"));
        Assert.That(saved.PasswordHash, Is.EqualTo("hashed_pwd"));

        var dtoResponse = result.Value;
        Assert.That(dtoResponse.Login, Is.EqualTo(saved.Login.Value));
        Assert.That(dtoResponse.Email, Is.EqualTo(saved.Email.Value));
        Assert.That(dtoResponse.AvatarUrl, Is.Empty); // в handler передаётся пустая строка
    }
}