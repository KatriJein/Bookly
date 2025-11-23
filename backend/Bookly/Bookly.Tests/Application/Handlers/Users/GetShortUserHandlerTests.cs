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
public class GetShortUserHandlerTests
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
        var handler = new GetShortUserHandler(_mediator, _db, _options);
        var query = new GetShortUserQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
        await _mediator.DidNotReceiveWithAnyArgs().Send(default!, default);
    }

    [Test]
    public async Task Handle_ReturnsDto_WithPresignedUrl_WhenUserHasAvatar()
    {
        // Arrange
        var user = User.Create(new CreateUserDto("shortLogin", "short@mail.com", "hash")).Value;
        typeof(User).GetProperty("AvatarKey")?.SetValue(user, "short_avatar");
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        const string expectedUrl = "https://cdn/avatar_short.jpg";
        _mediator.Send(Arg.Any<GetPresignedUrlQuery>(), Arg.Any<CancellationToken>())
            .Returns(expectedUrl);

        var handler = new GetShortUserHandler(_mediator, _db, _options);
        var query = new GetShortUserQuery(user.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Login, Is.EqualTo("shortLogin"));
        Assert.That(result.AvatarUrl, Is.EqualTo(expectedUrl));

        await _mediator.Received(1).Send(
            Arg.Is<GetPresignedUrlQuery>(q =>
                q.GetObjectPresinedUrlDto.Bucket == "bucket" &&
                q.GetObjectPresinedUrlDto.Key == "short_avatar"),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_ReturnsDto_WithEmptyAvatar_WhenUserHasNoAvatarKey()
    {
        // Arrange
        var user = User.Create(new CreateUserDto("nopic_login", "nopic@mail.com", "hash")).Value;
        typeof(User).GetProperty("AvatarKey")?.SetValue(user, null);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        _mediator.Send(Arg.Any<GetPresignedUrlQuery>(), Arg.Any<CancellationToken>())
            .Returns("");

        var handler = new GetShortUserHandler(_mediator, _db, _options);
        var query = new GetShortUserQuery(user.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Login, Is.EqualTo("nopic_login"));
        Assert.That(result.AvatarUrl, Is.EqualTo(""));
    }
}