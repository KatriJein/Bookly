using Bookly.Application.Handlers.BookCollections;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.BookCollection;
using Core.Dto.User;

namespace Bookly.Tests.Application.Handlers.BookCollections;

[TestFixture]
public class GetBookCollectionsBriefInfoHandlerTests
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
    public async Task Handle_ReturnsOnlyUserCollections_InCorrectOrder()
    {
        var userDto = new CreateUserDto("User1", "u1@mail.com", "hash");
        var user = User.Create(userDto).Value;
        var otherUserDto = new CreateUserDto("User2", "u2@mail.com", "hash");
        var otherUser = User.Create(otherUserDto).Value;
        _db.Users.AddRange(user, otherUser);
        await _db.SaveChangesAsync();

        var static1 = BookCollection.Create(new CreateBookCollectionDto("Читаю", true, user.Id), true).Value;
        var static2 = BookCollection.Create(new CreateBookCollectionDto("Хочу прочитать", true, user.Id), true).Value;
        var custom1 = BookCollection.Create(new CreateBookCollectionDto("Моя подборка", false, user.Id), false).Value;
        var custom2 = BookCollection.Create(new CreateBookCollectionDto("Авторское", false, user.Id), false).Value;

        var otherUserCollection = BookCollection.Create(new CreateBookCollectionDto("Чужая", false, otherUser.Id), false).Value;

        _db.BookCollections.AddRange(static1, static2, custom1, custom2, otherUserCollection);
        await _db.SaveChangesAsync();

        var handler = new GetBookCollectionsBriefInfoHandler(_db);
        var result = await handler.Handle(new GetBookCollectionsBriefInfoQuery(user.Id), CancellationToken.None);

        Assert.That(result.Count, Is.EqualTo(4));
        Assert.That(result.All(c => c.UserId == user.Id));

        var expectedOrder = result.OrderByDescending(c =>
            (c.Title == "Читаю" || c.Title == "Хочу прочитать"))
            .ThenBy(c => c.Title)
            .ToList();

        var actualOrder = result.ToList();
        Assert.That(actualOrder, Is.EqualTo(expectedOrder).Using<GetShortBookCollectionDto>((a, b) => a.Id == b.Id ? 0 : 1));
    }

    [Test]
    public async Task Handle_ReturnsEmptyList_WhenUserHasNoCollections()
    {
        var userDto = new CreateUserDto("EmptyUser", "e@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var handler = new GetBookCollectionsBriefInfoHandler(_db);
        var result = await handler.Handle(new GetBookCollectionsBriefInfoQuery(user.Id), CancellationToken.None);

        Assert.That(result, Is.Empty);
    }
}