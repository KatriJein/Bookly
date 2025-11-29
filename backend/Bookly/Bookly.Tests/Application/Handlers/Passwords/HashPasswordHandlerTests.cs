using Bookly.Application.Handlers.Passwords;
using Bookly.Application.Services.Passwords;
using Core;
using NSubstitute;

namespace Bookly.Tests.Application.Handlers.Passwords;

[TestFixture]
public class HashPasswordHandlerTests
{
    private IPasswordHasher _passwordHasher = null!;

    [SetUp]
    public void Setup()
    {
        _passwordHasher = Substitute.For<IPasswordHasher>();
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenPasswordFormatInvalid()
    {
        // Arrange: явно заведомо не подходящий пароль
        var handler = new HashPasswordHandler(_passwordHasher);
        var cmd = new HashPasswordCommand("abc"); // слишком короткий или без спецсимволов — зависит от PasswordRegex

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Does.Contain("Пароль не соответствует формату"));

        // Убедимся, что хешер при этом вообще не вызывался
        _passwordHasher.DidNotReceiveWithAnyArgs().HashPassword(default!);
    }

    [Test]
    public async Task Handle_ReturnsHashedPassword_WhenPasswordValid()
    {
        const string rawPassword = "ValidPass123!";
        const string expectedHash = "hashed_pwd";
        var handler = new HashPasswordHandler(_passwordHasher);
        
        _passwordHasher.HashPassword(rawPassword)
            .Returns(Result<string>.Success(expectedHash));
        
        var result = await handler.Handle(new HashPasswordCommand(rawPassword), CancellationToken.None);
        
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(expectedHash));
    }

    [Test]
    public async Task Handle_PropagatesFailure_FromHasher()
    {
        // Arrange
        const string password = "ValidPass123!";
        var handler = new HashPasswordHandler(_passwordHasher);

        _passwordHasher.HashPassword(password)
            .Returns(Result<string>.Failure("ошибка хеширования"));

        // Act
        var result = await handler.Handle(new HashPasswordCommand(password), CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Does.Contain("ошибка хеширования"));
    }
}