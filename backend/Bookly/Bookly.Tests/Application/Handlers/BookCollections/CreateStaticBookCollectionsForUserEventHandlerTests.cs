using Bookly.Application.Handlers.BookCollections;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Data;
using Moq;
using Serilog;

namespace Bookly.Tests.Application.Handlers.BookCollections;

[TestFixture]
public class CreateStaticBookCollectionsForUserEventHandlerTests
{
    private BooklyDbContext _db = null!;
    private Mock<ILogger> _loggerMock = null!;

    [SetUp]
    public void Setup()
    {
        _db = DatabaseUtils.CreateDbContext();
        _loggerMock = new Mock<ILogger>();
    }

    [TearDown]
    public void Teardown()
    {
        _db.Dispose();
    }

    [Test]
    public async Task Handle_CreatesFourStaticCollections_ForNewUser()
    {
        var userId = Guid.NewGuid();
        var userCreatedEvent = new UserCreatedEvent(userId);
        var handler = new CreateStaticBookCollectionsForUserEventHandler(_db, _loggerMock.Object);

        await handler.Handle(userCreatedEvent, CancellationToken.None);
        await _db.SaveChangesAsync();

        var collections = _db.BookCollections.ToList();

        Assert.That(collections, Has.Count.EqualTo(4));
        Assert.That(collections.All(c => c.IsStatic));
        Assert.That(collections.All(c => c.UserId == userId));

        var expectedNames = StaticBookCollectionsData.StaticBookCollectionsNames;
        var actualNames = collections.Select(c => c.Title).ToArray();
        Assert.That(actualNames, Is.EquivalentTo(expectedNames));
    }
}