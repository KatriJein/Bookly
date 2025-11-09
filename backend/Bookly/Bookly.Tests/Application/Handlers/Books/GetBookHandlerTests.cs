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
    public class GetBookHandlerTests
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
        public async Task Handle_ReturnsFullBookDto_WhenBookExists()
        {
            var genre = Genre.Create(new CreateGenreDto("Fiction", "Худлит")).Value;
            var author = Author.Create(new CreateAuthorDto("Толстой Л.Н.", "Лев Толстой")).Value;
            var publisher = Publisher.Create(new CreatePublisherDto("Издательство")).Value;

            var bookRes = Book.Create(new CreateBookDto(
                "Война и мир", "Описание", 4.8, 20000, "ru", "Издательство",
                1869, 1200, AgeRestriction.Everyone, "thumb", "ext1",
                Array.Empty<string>(), Array.Empty<string>())
            );
            var book = bookRes.Value;
            book.AddAuthors(new[] { author });
            book.AddGenres(new[] { genre });
            book.SetPublisher(publisher);

            _db.Books.Add(book);
            await _db.SaveChangesAsync();

            var handler = new GetBookHandler(_db);
            var result = await handler.Handle(new GetBookQuery(book.Id), CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Title, Is.EqualTo("Война и мир"));
            Assert.That(result.Authors.Select(a => a.DisplayName), Is.EquivalentTo(new[] { "Лев Толстой" }));
            Assert.That(result.Genres.Select(a => a.DisplayName), Is.EquivalentTo(new[] { "Худлит" }));
            Assert.That(result.AgeRestriction, Is.EqualTo("0+"));
            Assert.That(result.Description, Is.EqualTo("Описание"));
            Assert.That(result.Publisher, Is.EqualTo("Издательство"));
        }

        [Test]
        public async Task Handle_ReturnsNull_WhenBookDoesNotExist()
        {
            var handler = new GetBookHandler(_db);
            var result = await handler.Handle(new GetBookQuery(Guid.NewGuid()), CancellationToken.None);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Handle_ReturnsCorrectAgeRestrictionString()
        {
            var genre = Genre.Create(new CreateGenreDto("History", "История")).Value;
            var author = Author.Create(new CreateAuthorDto("Автор", "Автор")).Value;
            var publisher = Publisher.Create(new CreatePublisherDto("Издательство")).Value;
            
            var bookRes = Book.Create(new CreateBookDto(
                "Книга", "desc", 3, 10, "ru", "Изд", 2020, 500,
                AgeRestriction.Mature, "thumb", "ext2",
                Array.Empty<string>(), Array.Empty<string>()));
            var book = bookRes.Value;
            book.AddAuthors(new[] { author });
            book.AddGenres(new[] { genre });
            book.SetPublisher(publisher);

            _db.Books.Add(book);
            await _db.SaveChangesAsync();

            var handler = new GetBookHandler(_db);
            var dto = await handler.Handle(new GetBookQuery(book.Id), CancellationToken.None);

            Assert.That(dto, Is.Not.Null);
            Assert.That(dto!.AgeRestriction, Is.EqualTo("18+"));
        }
    }