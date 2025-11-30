
using Bookly.Application.Handlers.Books;
using Bookly.Application.Handlers.Rateable;
using Bookly.Application.Handlers.Ratings;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.Author;
using Core.Dto.Book;
using Core.Dto.Genre;
using Core.Enums;
using Core.Mappers;
using MediatR;
using Moq;

namespace Bookly.Tests.Application.Handlers.Books;

[TestFixture]
public class GetSimilarBooksHandlerTests
{
    private BooklyDbContext _dbContext = null!;
    private Mock<IMediator> _mediatorMock = null!;

    [SetUp]
    public void Setup()
    {
        _dbContext = DatabaseUtils.CreateDbContext();
        _mediatorMock = new Mock<IMediator>();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CalculateAverageRatingQuery<Book>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(4.0);
        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<ExcludeIrrelevantBooksAndEnrichRelevantWithDataCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ExcludeIrrelevantBooksAndEnrichRelevantWithDataCommand cmd, CancellationToken _) => cmd.Books);
    }

    [TearDown]
    public void Cleanup()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task One_Perfect_Match_Should_Be_First()
    {
        var baseBook = CreateBook("Base", 4.5, 10, "EN", 250, AgeRestriction.Teen, ["A1"], ["Fantasy", "Mystery"]);
        var perfect = CreateBook("Perfect", 4.0, 30, "EN", 260, AgeRestriction.Teen, ["A1"], ["Fantasy", "Mystery"]);
        var partial = CreateBook("Partial", 4.2, 15, "EN", 300, AgeRestriction.Teen, ["A2"], ["Fantasy"]);
        var unrelated = CreateBook("Unrelated", 5.0, 40, "DE", 100, AgeRestriction.Mature, ["B1"], ["Horror"]);
        _dbContext.Books.AddRange(baseBook, perfect, partial, unrelated);
        await _dbContext.SaveChangesAsync();

        var handler = new GetSimilarBooksHandler(_mediatorMock.Object, _dbContext);
        var query = new GetSimilarBooksQuery(baseBook.Id, new BookSimpleSearchSettingsDto(1, 10));
        var result = await handler.Handle(query, CancellationToken.None);

        var titlesByOrder = result.Select(x => x.Title).ToList();
        Assert.That(titlesByOrder.First(), Is.EqualTo("Perfect"));
        Assert.That(titlesByOrder.Last(), Is.EqualTo("Partial"));
    }

    [Test]
    public async Task Several_Partial_Matches_Should_Sort_By_Weight_And_Rating()
    {
        var baseBook = CreateBook("BookBase", 4.0, 10, "EN", 200, AgeRestriction.YoungAdult, ["A1", "A2"], ["Fantasy", "Adventure"]);
        var similar1 = CreateBook("Similar1", 5.0, 50, "EN", 210, AgeRestriction.YoungAdult, ["A1"], ["Fantasy"]);
        var similar2 = CreateBook("Similar2", 3.5, 10, "EN", 190, AgeRestriction.YoungAdult, ["A2"], ["Adventure"]);
        var similar3 = CreateBook("Similar3", 4.0, 20, "EN", 300, AgeRestriction.YoungAdult, ["B1"], ["Fantasy"]);
        _dbContext.Books.AddRange(baseBook, similar1, similar2, similar3);
        await _dbContext.SaveChangesAsync();

        var handler = new GetSimilarBooksHandler(_mediatorMock.Object, _dbContext);
        var query = new GetSimilarBooksQuery(baseBook.Id, new BookSimpleSearchSettingsDto(1, 10));
        var result = await handler.Handle(query, CancellationToken.None);
        var titlesByOrder = result.Select(x => x.Title).Distinct().ToList();
        Assert.That(titlesByOrder, Is.EqualTo(new string[] {"Similar1", "Similar3", "Similar2"}));
    }

    [Test]
    public async Task Larger_DataSet_Should_Return_Most_Relevant_First()
    {
        var baseBook = CreateBook("Core", 3.0, 5, "RU", 150, AgeRestriction.Children, ["C1"], ["SciFi"]);
        _dbContext.Books.Add(baseBook);

        for (int i = 0; i < 10; i++)
        {
            var lang = i % 2 == 0 ? "RU" : "EN";
            var genre = i % 3 == 0 ? "SciFi" : "Drama";
            var authors = i % 4 == 0 ? new[] { "C1" } : new[] { $"Author{i}" };
            var age = i % 2 == 0 ? AgeRestriction.Children : AgeRestriction.Mature;
            var book = CreateBook($"Book{i}", 3 + i * 0.1, 10 + i, lang, 150 + i, age, authors, new[] { genre });
            _dbContext.Books.Add(book);
        }

        await _dbContext.SaveChangesAsync();

        var handler = new GetSimilarBooksHandler(_mediatorMock.Object, _dbContext);
        var query = new GetSimilarBooksQuery(baseBook.Id, new BookSimpleSearchSettingsDto(1, 20));
        var result = await handler.Handle(query, CancellationToken.None);
        result = result.DistinctBy(b => b.Id).ToList();
        Assert.That(result.Count, Is.EqualTo(10));
        var best = result.First();
        Assert.That(best.Language, Is.EqualTo("RU"));
        Assert.That(best.Genres.Any(g => g.Name == "SciFi"));
        Assert.That(best.AgeRestriction, Is.EqualTo(EnumMapper.MapAgeRestrictionEnumToString(AgeRestriction.Children)));
    }

    [Test]
    public async Task Slight_Difference_In_Volume_Should_Not_Affect_Order()
    {
        var baseBook = CreateBook("Source", 4.8, 50, "EN", 270, AgeRestriction.Everyone, ["AA"], ["History"]);
        var closeSize = CreateBook("CloseSize", 4.0, 10, "EN", 260, AgeRestriction.Everyone, ["AA"], ["History"]);
        var farSize = CreateBook("FarSize", 4.0, 10, "EN", 500, AgeRestriction.Everyone, ["AA"], ["History"]);
        _dbContext.Books.AddRange(baseBook, closeSize, farSize);
        await _dbContext.SaveChangesAsync();

        var handler = new GetSimilarBooksHandler(_mediatorMock.Object, _dbContext);
        var query = new GetSimilarBooksQuery(baseBook.Id, new BookSimpleSearchSettingsDto(1, 10));
        var result = await handler.Handle(query, CancellationToken.None);
        Assert.That(result.First().Title, Is.EqualTo("CloseSize"));
        Assert.That(result.Last().Title, Is.EqualTo("FarSize"));
    }

    [Test]
    public async Task Matching_Language_But_Different_Authors_Should_Get_Lower_Weight()
    {
        var baseBook = CreateBook("LangBase", 3.0, 30, "FR", 200, AgeRestriction.YoungAdult, ["L1"], ["Drama"]);
        var langMatch = CreateBook("LangMatch", 3.0, 30, "FR", 200, AgeRestriction.YoungAdult, ["L2"], ["Comedy"]);
        var fullMatch = CreateBook("FullMatch", 3.0, 30, "FR", 200, AgeRestriction.YoungAdult, ["L1"], ["Drama"]);
        _dbContext.Books.AddRange(baseBook, langMatch, fullMatch);
        await _dbContext.SaveChangesAsync();

        var handler = new GetSimilarBooksHandler(_mediatorMock.Object, _dbContext);
        var query = new GetSimilarBooksQuery(baseBook.Id, new BookSimpleSearchSettingsDto(1, 10));
        var result = await handler.Handle(query, CancellationToken.None);
        var ordered = result.Select(r => r.Title).ToList();
        Assert.That(ordered.First(), Is.EqualTo("FullMatch"));
        Assert.That(ordered.Last(), Is.EqualTo("LangMatch"));
    }

    [Test]
    public async Task Exact_AgeRestriction_Match_Should_Influence_Weight()
    {
        var baseBook = CreateBook("AgeBase", 4.0, 10, "EN", 220, AgeRestriction.Teen, ["A1"], ["Drama"]);
        var sameAge = CreateBook("SameAge", 3.5, 20, "EN", 220, AgeRestriction.Teen, ["A1"], ["Drama"]);
        var differentAge = CreateBook("DiffAge", 3.5, 20, "EN", 220, AgeRestriction.Mature, ["A1"], ["Drama"]);
        _dbContext.Books.AddRange(baseBook, sameAge, differentAge);
        await _dbContext.SaveChangesAsync();

        var handler = new GetSimilarBooksHandler(_mediatorMock.Object, _dbContext);
        var query = new GetSimilarBooksQuery(baseBook.Id, new BookSimpleSearchSettingsDto(1, 10));
        var result = await handler.Handle(query, CancellationToken.None);

        var ordered = result.Select(r => r.Title).ToList();
        Assert.That(ordered.First(), Is.EqualTo("SameAge"));
        Assert.That(ordered.Last(), Is.EqualTo("DiffAge"));
    }

    [Test]
    public async Task Returns_EmptyList_If_Book_NotFound()
    {
        var handler = new GetSimilarBooksHandler(_mediatorMock.Object, _dbContext);
        var query = new GetSimilarBooksQuery(Guid.NewGuid(), new BookSimpleSearchSettingsDto(1, 10));
        var result = await handler.Handle(query, CancellationToken.None);
        Assert.That(result, Is.Empty);
    }

    private static Book CreateBook(string title, double rating, int ratingsCount, string language, int pages, AgeRestriction age, string[] authors, string[] genres)
    {
        var dto = new CreateBookDto(title, null, rating, ratingsCount, language, "Publisher", 2000, pages, age, null, Guid.NewGuid().ToString(), authors, genres);
        var result = Book.Create(dto);
        var book = result.Value;
        book.AddAuthors(authors.Select(a => Author.Create(new CreateAuthorDto(a, a)).Value));
        book.AddGenres(genres.Select(g => Genre.Create(new CreateGenreDto(g, g)).Value));
        return book;
    }
}