using Bookly.Application.Handlers.Authors;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.Author;

namespace Bookly.Tests.Application.Handlers.Authors;

public class GetAllAuthorsHandlerTests
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
    public async Task Handle_ReturnsAllAuthors_AsDtos()
    {
        _db.Authors.AddRange(
            Author.Create(new CreateAuthorDto("Пушкин А.С.", "Александр Пушкин")).Value,
            Author.Create(new CreateAuthorDto("Толстой Л.Н.", "Лев Толстой")).Value,
            Author.Create(new CreateAuthorDto("Достоевский Ф.М.", "Федор Достоевский")).Value
        );
        await _db.SaveChangesAsync();

        var handler = new GetAllAuthorsHandler(_db);
        var result = await handler.Handle(new GetAllAuthorsQuery(new AuthorSearchSettingsDto()), CancellationToken.None);

        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result[0].FullName, Is.EqualTo("Пушкин А.С."));
    }

    [Test]
    public async Task Handle_ReturnsAllAuthors_AsDtos_WithFilter()
    {
        _db.Authors.AddRange(
            Author.Create(new CreateAuthorDto("Пушкин А.С.", "Александр Пушкин")).Value,
            Author.Create(new CreateAuthorDto("Пушкин Сергей", "Сергей Пушкин")).Value,
            Author.Create(new CreateAuthorDto("Достоевский Ф.М.", "Федор Достоевский")).Value);
        await _db.SaveChangesAsync();
        
        var handler = new GetAllAuthorsHandler(_db);
        var result = await handler.Handle(new GetAllAuthorsQuery(new AuthorSearchSettingsDto("пуш")), CancellationToken.None);
        
        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Handle_ReturnsEmptyList_WhenNoAuthorsExist()
    {
        var handler = new GetAllAuthorsHandler(_db);
        var result = await handler.Handle(new GetAllAuthorsQuery(new AuthorSearchSettingsDto()), CancellationToken.None);
        Assert.That(result, Is.Empty);
    }
}