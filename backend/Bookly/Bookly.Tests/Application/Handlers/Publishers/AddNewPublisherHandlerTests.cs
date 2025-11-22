using Bookly.Application.Handlers.Publishers;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.Publisher;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Bookly.Tests.Application.Handlers.Publishers;

[TestFixture]
public class AddNewPublisherHandlerTests
{
    private BooklyDbContext _db = null!;
    private Serilog.ILogger _logger = null!;

    [SetUp]
    public void Setup()
    {
        _db = DatabaseUtils.CreateDbContext();
        _logger = Substitute.For<Serilog.ILogger>();
    }

    [TearDown]
    public void Teardown()
    {
        _db.Dispose();
    }

    [Test]
    public async Task Handle_ReturnsExistingPublisher_WhenAlreadyExists()
    {
        // Arrange
        var existing = Publisher.Create(new CreatePublisherDto("Издатель")).Value;
        _db.Publishers.Add(existing);
        await _db.SaveChangesAsync();

        var cmd = new AddNewPublisherCommand(new CreatePublisherDto("Издатель"));
        var handler = new AddNewPublisherHandler(_db, _logger);
        
        var result = await handler.Handle(cmd, CancellationToken.None);
        
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Id, Is.EqualTo(existing.Id));
        
        Assert.That(await _db.Publishers.CountAsync(), Is.EqualTo(1));
    }

    [Test]
    public async Task Handle_CreatesNewPublisher_WhenNotExists()
    {
        // Arrange
        var cmd = new AddNewPublisherCommand(new CreatePublisherDto("Новое издательство"));
        var handler = new AddNewPublisherHandler(_db, _logger);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(await _db.Publishers.CountAsync(), Is.EqualTo(1));

        var saved = await _db.Publishers.FirstAsync();
        Assert.That(saved.Name, Is.EqualTo("Новое издательство"));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenPublisherCreationFails()
    {
        // Arrange
        var invalidDto = new CreatePublisherDto(""); // допустим, пустое имя невалидно
        var cmd = new AddNewPublisherCommand(invalidDto);
        var handler = new AddNewPublisherHandler(_db, _logger);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(await _db.Publishers.CountAsync(), Is.EqualTo(0));
    }
}