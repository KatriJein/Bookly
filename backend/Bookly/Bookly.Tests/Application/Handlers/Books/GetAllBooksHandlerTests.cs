using Bookly.Application.Handlers.Books;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.Author;
using Core.Dto.Book;
using Core.Dto.Genre;
using Core.Dto.Publisher;
using Core.Enums;

namespace Bookly.Tests.Application.Handlers.Books;

    [TestFixture]
    public class GetAllBooksHandlerTests
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

        private async Task SeedBooksAsync()
        {
            var author1 = Author.Create(new CreateAuthorDto("Пушкин А.С.", "Александр Пушкин")).Value;
            var author2 = Author.Create(new CreateAuthorDto("Толстой Л.Н.", "Лев Толстой")).Value;
            var genre1 = Genre.Create(new CreateGenreDto("Фантастика", "Фантастика")).Value;
            var genre2 = Genre.Create(new CreateGenreDto("Роман", "Роман")).Value;

            var pub1 = Publisher.Create(new CreatePublisherDto("Издательство 1")).Value;
            var pub2 = Publisher.Create(new CreatePublisherDto("Издательство 2")).Value;

            var b1 = Book.Create(new CreateBookDto("Онегин", "desc", 4.1, 300, "ru", "Издательство 1", 1833, 400,
                AgeRestriction.Everyone, null, "1", Array.Empty<string>(), Array.Empty<string>())).Value;
            b1.AddAuthors(new[] { author1 });
            b1.AddGenres(new[] { genre2 });
            b1.SetPublisher(pub1);

            var b2 = Book.Create(new CreateBookDto("Война и мир", "desc", 4.8, 5000, "ru", "Издательство 2", 1869, 1300,
                AgeRestriction.Everyone, null, "2", Array.Empty<string>(), Array.Empty<string>())).Value;
            b2.AddAuthors(new[] { author2 });
            b2.AddGenres(new[] { genre2 });
            b2.SetPublisher(pub2);

            var b3 = Book.Create(new CreateBookDto("Будущее", "desc", 3.0, 100, "ru", "Издательство 1", 2020, 250,
                AgeRestriction.Teen, null, "3", Array.Empty<string>(), Array.Empty<string>())).Value;
            b3.AddAuthors(new[] { author1 });
            b3.AddGenres(new[] { genre1 });
            b3.SetPublisher(pub1);

            _db.Books.AddRange(b1, b2, b3);
            await _db.SaveChangesAsync();
        }

        [Test]
        public async Task Handle_ReturnsAllBooks_WhenNoFilters()
        {
            await SeedBooksAsync();
            var settings = new BookSearchSettingsDto();
            var handler = new GetAllBooksHandler(_db);

            var result = await handler.Handle(new GetAllBooksQuery(settings), CancellationToken.None);

            Assert.That(result, Has.Count.EqualTo(3));
        }

        [Test]
        public async Task Handle_FiltersByTitle()
        {
            await SeedBooksAsync();
            var settings = new BookSearchSettingsDto(SearchByTitle: "война");
            var handler = new GetAllBooksHandler(_db);

            var result = await handler.Handle(new GetAllBooksQuery(settings), CancellationToken.None);

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Title.ToLower(), Does.Contain("война"));
        }
        
        [Test]
        public async Task Handle_FiltersByPublisher()
        {
            await SeedBooksAsync();
            var settings = new BookSearchSettingsDto(SearchByPublisher: "издательство 1");
            var handler = new GetAllBooksHandler(_db);

            var result = await handler.Handle(new GetAllBooksQuery(settings), CancellationToken.None);

            Assert.That(result, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task Handle_FiltersByGenre()
        {
            await SeedBooksAsync();
            var settings = new BookSearchSettingsDto(SearchByGenres: new[] { "Роман" });
            var handler = new GetAllBooksHandler(_db);

            var result = await handler.Handle(new GetAllBooksQuery(settings), CancellationToken.None);

            Assert.That(result.All(b => b.Genres.Contains("Роман")));
        }

        [Test]
        public async Task Handle_FiltersByAuthor()
        {
            await SeedBooksAsync();
            var settings = new BookSearchSettingsDto(SearchByAuthors: new[] { "Толстой Л.Н." });
            var handler = new GetAllBooksHandler(_db);

            var result = await handler.Handle(new GetAllBooksQuery(settings), CancellationToken.None);

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Authors.Contains("Лев Толстой"));
        }

        [Test]
        public async Task Handle_FiltersByRating()
        {
            await SeedBooksAsync();
            var settings = new BookSearchSettingsDto(SearchByRating: 4.5);
            var handler = new GetAllBooksHandler(_db);

            var result = await handler.Handle(new GetAllBooksQuery(settings), CancellationToken.None);
            Assert.That(result.All(r => r.Rating >= 4.5));
        }

        [Test]
        public async Task Handle_FiltersByTitleAndAuthor()
        {
            await SeedBooksAsync();
            var settings = new BookSearchSettingsDto(SearchByTitle: "война", SearchByAuthors: new[] { "Толстой Л.Н." });
            var handler = new GetAllBooksHandler(_db);

            var result = await handler.Handle(new GetAllBooksQuery(settings), CancellationToken.None);

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Title.ToLower(), Does.Contain("война"));
            Assert.That(result.First().Authors.Contains("Лев Толстой"));
        }
        
        [Test]
        public async Task Handle_FiltersByPublisherAndGenre()
        {
            await SeedBooksAsync();
            var settings = new BookSearchSettingsDto(
                SearchByPublisher: "издательство 1",
                SearchByGenres: new[] { "Фантастика" }
            );

            var handler = new GetAllBooksHandler(_db);
            var result = await handler.Handle(new GetAllBooksQuery(settings), CancellationToken.None);

            Assert.That(result, Has.Count.EqualTo(1));
            var book = result.First();
            Assert.That(book.Genres.Contains("Фантастика"));
        }

        [Test]
        public async Task Handle_SortsByTitleDescending()
        {
            await SeedBooksAsync();
            var settings = new BookSearchSettingsDto(BooksOrderOption: BooksOrderOption.ByTitleDescending);
            var handler = new GetAllBooksHandler(_db);

            var result = await handler.Handle(new GetAllBooksQuery(settings), CancellationToken.None);

            var titles = result.Select(r => r.Title).ToList();
            var sorted = titles.OrderByDescending(t => t).ToList();
            Assert.That(titles, Is.EqualTo(sorted));
        }

        [Test]
        public async Task Handle_SortsByRatingAscending()
        {
            await SeedBooksAsync();
            var settings = new BookSearchSettingsDto(BooksOrderOption: BooksOrderOption.ByRatingAscending);
            var handler = new GetAllBooksHandler(_db);

            var result = await handler.Handle(new GetAllBooksQuery(settings), CancellationToken.None);
            var ratings = result.Select(r => r.Rating).ToList();
            var sorted = ratings.OrderBy(r => r).ToList();
            Assert.That(ratings, Is.EqualTo(sorted));
        }
        
        [Test]
        public async Task Handle_PaginatesProperly()
        {
            await SeedBooksAsync();
            var settingsPage1 = new BookSearchSettingsDto(Limit: 2, Page: 1, BooksOrderOption: BooksOrderOption.ByTitleAscending);
            var settingsPage2 = new BookSearchSettingsDto(Limit: 2, Page: 2, BooksOrderOption: BooksOrderOption.ByTitleAscending);
            var handler = new GetAllBooksHandler(_db);

            var page1 = await handler.Handle(new GetAllBooksQuery(settingsPage1), CancellationToken.None);
            var page2 = await handler.Handle(new GetAllBooksQuery(settingsPage2), CancellationToken.None);

            Assert.That(page1.Count, Is.EqualTo(2));
            Assert.That(page2.Count, Is.EqualTo(1));
            Assert.That(page1.Select(b => b.Id).Intersect(page2.Select(b => b.Id)), Is.Empty);
        }

        [Test]
        public async Task Handle_FiltersAndSortsTogether()
        {
            await SeedBooksAsync();
            var settings = new BookSearchSettingsDto(SearchByGenres: new[] { "Роман" }, BooksOrderOption: BooksOrderOption.ByRatingDescending);
            var handler = new GetAllBooksHandler(_db);

            var result = await handler.Handle(new GetAllBooksQuery(settings), CancellationToken.None);
            Assert.That(result.All(r => r.Genres.Contains("Роман")));
            var ratings = result.Select(r => r.Rating).ToList();
            var sorted = ratings.OrderByDescending(r => r).ToList();
            Assert.That(ratings, Is.EqualTo(sorted));
        }
    }