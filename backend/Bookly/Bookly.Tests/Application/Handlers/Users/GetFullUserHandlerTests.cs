using Bookly.Application.Handlers.Files;
using Bookly.Application.Handlers.Users;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.User;
using Core.Options;
using MediatR;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Bookly.Tests.Application.Handlers.Users;

[TestFixture]
public class GetFullUserHandlerTests
{
    private IMediator _mediator = null!;
    private BooklyDbContext _db = null!;
    private IOptionsSnapshot<BooklyOptions> _options = null!;

    [SetUp]
    public void Setup()
    {
        _mediator = Substitute.For<IMediator>();
        _db = DatabaseUtils.CreateDbContext();
        _options = Substitute.For<IOptionsSnapshot<BooklyOptions>>();
        _options.Value.Returns(new BooklyOptions { BooklyFilesStorageBucketName = "bucket" });
    }

    [TearDown]
    public void Teardown()
    {
        _db.Dispose();
    }

    [Test]
    public async Task Handle_ReturnsNull_WhenUserNotFound()
    {
        // Arrange
        var handler = new GetFullUserHandler(_mediator, _db, _options);
        var query = new GetFullUserQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
        // Mediатор не должен вызываться
        await _mediator.DidNotReceiveWithAnyArgs().Send(default!, default);
    }

    [Test]
    public async Task Handle_ReturnsDto_WithPresignedUrl_WhenUserHasAvatar()
    {
        // Arrange
        var user = User.Create(new CreateUserDto("login", "test@mail.com", "hash")).Value;
        typeof(User).GetProperty("AvatarKey")?.SetValue(user, "avatar_key");
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        const string expectedUrl = "https://cdn/test/avatar.jpg";
        _mediator.Send(Arg.Any<GetPresignedUrlQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedUrl);

        var handler = new GetFullUserHandler(_mediator, _db, _options);
        var query = new GetFullUserQuery(user.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Login, Is.EqualTo("login"));
        Assert.That(result.Email, Is.EqualTo("test@mail.com"));
        Assert.That(result.AvatarUrl, Is.EqualTo(expectedUrl));

        await _mediator.Received(1).Send(
            Arg.Is<GetPresignedUrlQuery>(q =>
                q.GetObjectPresinedUrlDto.Bucket == "bucket" &&
                q.GetObjectPresinedUrlDto.Key == "avatar_key"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_ReturnsDto_WithEmptyUrl_WhenUserHasNoAvatar()
    {
        // Arrange
        var user = User.Create(new CreateUserDto("login2", "mail2@mail.com", "pwd")).Value;
        typeof(User).GetProperty("AvatarKey")?.SetValue(user, null);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        _mediator.Send(Arg.Any<GetPresignedUrlQuery>(), Arg.Any<CancellationToken>())
            .Returns(""); // медиатор вернул пустую ссылку

        var handler = new GetFullUserHandler(_mediator, _db, _options);
        var query = new GetFullUserQuery(user.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Login, Is.EqualTo("login2"));
        Assert.That(result.AvatarUrl, Is.Empty);
    }
}