using Bookly.Application.Chains.LoginChain;
using Bookly.Application.Handlers.Auth;
using Bookly.Application.Handlers.Files;
using Bookly.Application.Services.Passwords;
using Bookly.Domain.Models;
using Core.Dto.User;
using Core.Options;
using MediatR;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Bookly.Tests.Application.Handlers.Users;

[TestFixture]
public class AuthenthicationHandlerTests
{
    private IMediator _mediator = null!;
    private ILoginChain _loginChain = null!;
    private IPasswordHasher _passwordHasher = null!;
    private IOptionsSnapshot<BooklyOptions> _options = null!;

    private static readonly BooklyOptions DefaultOptions = new()
    {
        BooklyFilesStorageBucketName = "bucket"
    };

    [SetUp]
    public void Setup()
    {
        _mediator = Substitute.For<IMediator>();
        _loginChain = Substitute.For<ILoginChain>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _options = Substitute.For<IOptionsSnapshot<BooklyOptions>>();
        _options.Value.Returns(DefaultOptions);
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenUserNotFound()
    {
        // Arrange
        var request = new AuthenthicationCommand(new AuthRequestDto("notfound", "pwd"));

        _loginChain.FindUserByLoginAsync("notfound", Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var handler = new AuthenthicationHandler(_mediator, _loginChain, _passwordHasher, _options);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Does.Contain("Не найден пользователь"));
        _passwordHasher.DidNotReceiveWithAnyArgs().Verify(default!, default!);
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenPasswordIncorrect()
    {
        // Arrange
        var user = BuildFakeUser("user1", "email@example.com", "hash123", "key1");
        var request = new AuthenthicationCommand(new AuthRequestDto("user1", "badpwd"));

        _loginChain.FindUserByLoginAsync("user1", Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.Verify("badpwd", "hash123").Returns(false);

        var handler = new AuthenthicationHandler(_mediator, _loginChain, _passwordHasher, _options);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Does.Contain("Некорректный логин или пароль"));
    }

    [Test]
    public async Task Handle_ReturnsSuccess_WhenAllValid()
    {
        // Arrange
        var user = BuildFakeUser("user1", "email@example.com", "hash123", "avatarKey");
        var expectedUrl = "https://cdn/avatar.jpg";
        var request = new AuthenthicationCommand(new AuthRequestDto("user1", "correctPwd"));

        _loginChain.FindUserByLoginAsync("user1", Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.Verify("correctPwd", "hash123").Returns(true);
        _mediator.Send(Arg.Any<GetPresignedUrlQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedUrl);

        var handler = new AuthenthicationHandler(_mediator, _loginChain, _passwordHasher, _options);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);

        var dto = result.Value;
        Assert.That(dto.Login, Is.EqualTo("user1"));
        Assert.That(dto.Email, Is.EqualTo("email@example.com"));
        Assert.That(dto.AvatarUrl, Is.EqualTo(expectedUrl));

        await _mediator.Received(1).Send(
            Arg.Is<GetPresignedUrlQuery>(q =>
                q.GetObjectPresinedUrlDto.Bucket == "bucket" &&
                q.GetObjectPresinedUrlDto.Key == "avatarKey"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_ReturnsSuccess_WithEmptyAvatarKey()
    {
        // Arrange: пользователь без аватара
        var user = BuildFakeUser("user1", "mail@example.com", "hash123", null);
        var request = new AuthenthicationCommand(new AuthRequestDto("user1", "pwd"));

        _loginChain.FindUserByLoginAsync("user1", Arg.Any<CancellationToken>())
            .Returns(user);
        _passwordHasher.Verify("pwd", "hash123").Returns(true);
        _mediator.Send(Arg.Any<GetPresignedUrlQuery>(), Arg.Any<CancellationToken>())
            .Returns(""); // внутри handler пустая ссылка возможна

        var handler = new AuthenthicationHandler(_mediator, _loginChain, _passwordHasher, _options);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.AvatarUrl, Is.EqualTo(""));
    }

    [Test]
    public async Task Handle_ReturnsSuccess_WhenUserFoundByEmail()
    {
        // Arrange
        var emailLogin = "user@example.com";
        var user = BuildFakeUser("normalLogin", emailLogin, "secureHash", "avatarKeyEmail");
        var expectedUrl = "https://cdn/emailAvatar.jpg";

        var request = new AuthenthicationCommand(new AuthRequestDto(emailLogin, "pwd123"));

        _loginChain.FindUserByLoginAsync(emailLogin, Arg.Any<CancellationToken>())
            .Returns(user);

        _passwordHasher.Verify("pwd123", "secureHash").Returns(true);
        _mediator.Send(Arg.Any<GetPresignedUrlQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedUrl);

        var handler = new AuthenthicationHandler(_mediator, _loginChain, _passwordHasher, _options);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        var dto = result.Value;

        Assert.That(dto.Email, Is.EqualTo(emailLogin));
        Assert.That(dto.Login, Is.EqualTo("normalLogin"));
        Assert.That(dto.AvatarUrl, Is.EqualTo(expectedUrl));

        await _loginChain.Received(1)
            .FindUserByLoginAsync(emailLogin, Arg.Any<CancellationToken>());
    }

    private static User BuildFakeUser(string login, string email, string hash, string? avatarKey)
    {
        var user = User.Create(new CreateUserDto(login, email, hash)).Value;
        typeof(User).GetProperty("AvatarKey")?.SetValue(user, avatarKey);
        return user;
    }
}