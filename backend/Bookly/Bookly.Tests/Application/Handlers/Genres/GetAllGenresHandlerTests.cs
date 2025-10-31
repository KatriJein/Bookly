using Bookly.Application.Handlers.Genres;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.Genre;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Tests.Application.Handlers.Genres;

[TestFixture]
public class GetAllGenresHandlerTests
{
    private BooklyDbContext _dbContext = null!;

    [SetUp]
    public void Setup()
    {
        _dbContext = DatabaseUtils.CreateDbContext();
    }

    [TearDown]
    public void Teardown()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task Handle_ReturnsAllGenres_AsDtos()
    {
        var fiction = Genre.Create(new CreateGenreDto("Fiction", "Художественная литература")).Value;
        var history = Genre.Create(new CreateGenreDto("History", "История")).Value;
        var science = Genre.Create(new CreateGenreDto("Science", "Наука")).Value;

        _dbContext.Genres.AddRange(fiction, history, science);
        await _dbContext.SaveChangesAsync();

        var handler = new GetAllGenresHandler(_dbContext);

        // Act
        var result = await handler.Handle(new GetAllGenresQuery(), CancellationToken.None);

        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result[0].Name, Is.EqualTo("Fiction"));
        Assert.That(result[1].DisplayName, Is.EqualTo("История"));
    }

    [Test]
    public async Task Handle_ReturnsEmptyList_WhenNoGenresExist()
    {
        // Arrange
        var handler = new GetAllGenresHandler(_dbContext);

        // Act
        var result = await handler.Handle(new GetAllGenresQuery(), CancellationToken.None);

        // Assert
        Assert.That(result, Is.Empty);
    }
}