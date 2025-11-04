using Bookly.Application.Handlers.Users;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.User;

namespace Bookly.Tests.Application.Handlers.Users;

[TestFixture]
public class GetUsersHandlerTests
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
    public async Task Handle_ReturnsAllUsers_AsShortDto()
    {
        // Arrange
        var user1 = User.Create(new CreateUserDto("login1", "mail1@mail.com", "hash1")).Value;
        typeof(User).GetProperty("AvatarKey")?.SetValue(user1, "key1");

        var user2 = User.Create(new CreateUserDto("login2", "mail2@mail.com", "hash2")).Value;
        typeof(User).GetProperty("AvatarKey")?.SetValue(user2, "key2");

        _db.Users.AddRange(user1, user2);
        await _db.SaveChangesAsync();

        var handler = new GetUsersHandler(_db);

        // Act
        var result = await handler.Handle(new GetUsersQuery(), CancellationToken.None);

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));

        var logins = result.Select(r => r.Login).ToHashSet();
        Assert.That(logins, Does.Contain("login1").And.Contain("login2"));

        var urls = result.Select(r => r.AvatarUrl).ToList();
        Assert.That(urls, Does.Contain("key1"));
        Assert.That(urls, Does.Contain("key2"));
    }

    [Test]
    public async Task Handle_ReturnsEmptyList_WhenNoUsers()
    {
        // Arrange
        var handler = new GetUsersHandler(_db);

        // Act
        var result = await handler.Handle(new GetUsersQuery(), CancellationToken.None);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_MapsUserWithoutAvatar_ToEmptyUrl()
    {
        // Arrange
        var user = User.Create(new CreateUserDto("noAvatar", "none@mail.com", "pwd")).Value;
        typeof(User).GetProperty("AvatarKey")?.SetValue(user, null);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var handler = new GetUsersHandler(_db);

        // Act
        var result = await handler.Handle(new GetUsersQuery(), CancellationToken.None);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        var dto = result.Single();
        Assert.That(dto.Login, Is.EqualTo("noAvatar"));
        Assert.That(dto.AvatarUrl, Is.Empty);
    }
}