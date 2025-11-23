using Bookly.Application.Handlers.BookCollections;
using Bookly.Application.Handlers.Files;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.BookCollection;
using Core.Dto.User;
using Core.Options;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;

namespace Bookly.Tests.Application.Handlers.BookCollections;

[TestFixture]
public class GetBookCollectionsHandlerTests
{
    private BooklyDbContext _db = null!;
    private Mock<IMediator> _mediatorMock = null!;
    private Mock<IOptionsSnapshot<BooklyOptions>> _optionsMock = null!;

    [SetUp]
    public void Setup()
    {
        _db = DatabaseUtils.CreateDbContext();
        _mediatorMock = new Mock<IMediator>();
        _optionsMock = new Mock<IOptionsSnapshot<BooklyOptions>>();
        _optionsMock.Setup(o => o.Value).Returns(new BooklyOptions { BooklyFilesStorageBucketName = "bucket" });
    }

    [TearDown]
    public void Teardown()
    {
        _db.Dispose();
    }

    [Test]
    public async Task Handle_ReturnsSelfCollections_InCorrectOrder()
    {
        var userDto = new CreateUserDto("SelfUser", "user@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var staticCollection = BookCollection.Create(new CreateBookCollectionDto("Избранное", true, user.Id), true).Value;
        var dynamicCollection = BookCollection.Create(new CreateBookCollectionDto("Моя подборка", true, user.Id), false).Value;
        dynamicCollection.Actualize();

        _db.BookCollections.AddRange(staticCollection, dynamicCollection);
        await _db.SaveChangesAsync();

        var handler = new GetBookCollectionsHandler(_mediatorMock.Object, _db, _optionsMock.Object);

        var query = new GetBookCollectionsQuery(new BookCollectionSearchSettingsDto(Page: 1, Limit: 10), user.Id);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.All(c => c.UserId == user.Id));
        Assert.That(result[0].IsStatic);
        Assert.That(result[1].IsStatic, Is.False);
    }

    [Test]
    public async Task Handle_ReturnsPublicNonStaticCollections_WhenUserIdNull()
    {
        var userDto = new CreateUserDto("PublicUser", "p@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);

        var c1 = BookCollection.Create(new CreateBookCollectionDto("Коллекция 1", true, user.Id), false).Value;
        c1.SetIsPublic(true);
        c1.SetIsStatic(false);
        c1.SetUserId(user.Id);
        c1.SetCoverUrl("url1");
        typeof(BookCollection).GetProperty(nameof(BookCollection.Rating))!.SetValue(c1, 4.0);
        typeof(BookCollection).GetProperty(nameof(BookCollection.RatingsCount))!.SetValue(c1, 5);

        var c2 = BookCollection.Create(new CreateBookCollectionDto("Коллекция 2", true, user.Id), false).Value;
        c2.SetIsPublic(true);
        c2.SetIsStatic(false);
        c2.SetUserId(user.Id);
        c2.SetCoverUrl("url2");
        typeof(BookCollection).GetProperty(nameof(BookCollection.Rating))!.SetValue(c2, 4.8);
        typeof(BookCollection).GetProperty(nameof(BookCollection.RatingsCount))!.SetValue(c2, 20);

        _db.BookCollections.AddRange(c1, c2);
        await _db.SaveChangesAsync();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetPresignedUrlQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("avatar_url");

        var handler = new GetBookCollectionsHandler(_mediatorMock.Object, _db, _optionsMock.Object);

        var query = new GetBookCollectionsQuery(new BookCollectionSearchSettingsDto(Page: 1, Limit: 10), null);
        var result = await handler.Handle(query, CancellationToken.None);

        Assert.That(result.All(c => c.IsPublic && c.IsStatic == false));
        Assert.That(result.First().Title, Is.EqualTo("Коллекция 2"));
        Assert.That(result[0].UserInfo.Login, Is.EqualTo(user.Login.Value));
        Assert.That(result[0].UserInfo.AvatarUrl, Is.EqualTo("avatar_url"));
    }

    [Test]
    public async Task Handle_ReturnsEmptyList_WhenNoCollectionsMatch()
    {
        var handler = new GetBookCollectionsHandler(_mediatorMock.Object, _db, _optionsMock.Object);
        var query = new GetBookCollectionsQuery(new BookCollectionSearchSettingsDto(), null);

        var result = await handler.Handle(query, CancellationToken.None);
        Assert.That(result, Is.Empty);
    }
}