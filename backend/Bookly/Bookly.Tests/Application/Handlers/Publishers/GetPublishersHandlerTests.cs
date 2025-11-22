using Bookly.Application.Handlers.Publishers;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.Publisher;

namespace Bookly.Tests.Application.Handlers.Publishers;

[TestFixture]
public class GetPublishersHandlerTests
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
    public async Task Handle_ReturnsAllPublishers_SortedByName()
    {
        // Arrange
        var pub1 = Publisher.Create(new CreatePublisherDto("Молодая гвардия")).Value;
        var pub2 = Publisher.Create(new CreatePublisherDto("Азбука")).Value;
        var pub3 = Publisher.Create(new CreatePublisherDto("Эксмо")).Value;

        _db.Publishers.AddRange(pub1, pub2, pub3);
        await _db.SaveChangesAsync();

        var handler = new GetPublishersHandler(_db);

        // Act
        var result = await handler.Handle(new GetPublishersQuery(), CancellationToken.None);

        // Assert: проверяем количество и сортировку
        Assert.That(result, Has.Count.EqualTo(3));

        var names = result.Select(r => r.Name).ToList();
        var sorted = names.OrderBy(n => n).ToList();
        Assert.That(names, Is.EqualTo(sorted));
    }

    [Test]
    public async Task Handle_ReturnsEmptyList_WhenNoPublishers()
    {
        // Arrange
        var handler = new GetPublishersHandler(_db);

        // Act
        var result = await handler.Handle(new GetPublishersQuery(), CancellationToken.None);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_MapsPublisherEntity_ToDtoCorrectly()
    {
        // Arrange
        var pub = Publisher.Create(new CreatePublisherDto("Рипол Классик")).Value;
        _db.Publishers.Add(pub);
        await _db.SaveChangesAsync();

        var handler = new GetPublishersHandler(_db);

        // Act
        var result = await handler.Handle(new GetPublishersQuery(), CancellationToken.None);
        var dto = result.Single();

        // Assert
        Assert.That(dto.Name, Is.EqualTo(pub.Name));
    }
}