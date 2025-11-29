using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Bookly.Application.Handlers.Books;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.Book;
using Core.Dto.BookCollection;
using Core.Dto.Genre;
using Core.Dto.Recommendation;
using Core.Dto.User;
using Core.Dto.UserGenrePreference;
using Core.Enums;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Bookly.Tests
{
    [TestFixture]
    public class ExcludeIrrelevantBooksHandlerTests
    {
        private BooklyDbContext _dbContext = null!;
        private ExcludeIrrelevantBooksHandler _handler = null!;
        private Guid _userId;

        [SetUp]
        public async Task SetUp()
        {
            _dbContext = DatabaseUtils.CreateDbContext();
            _handler = new ExcludeIrrelevantBooksHandler(_dbContext);

            // Пользователь создаётся через DTO и фабрику
            var createUserDto = new CreateUserDto("testerLogin", "tester@example.com", "P@ssword1");
            var userRes = User.Create(createUserDto);
            Assert.That(userRes.IsSuccess);

            await _dbContext.Users.AddAsync(userRes.Value);
            await _dbContext.SaveChangesAsync();

            _userId = userRes.Value.Id;
        }

        [TearDown]
        public void TearDown() => _dbContext.Dispose();

        // ---------- TESTS ----------

        [Test]
        public async Task AnonymousUser_ReturnsSameList()
        {
            var books = CreateBooks(3);

            var result = await _handler.Handle(
                new ExcludeIrrelevantBooksCommand(books, null),
                CancellationToken.None);

            Assert.That(result, Has.Count.EqualTo(books.Count));
        }

        [Test]
        public async Task RatedBooks_AreExcluded()
        {
            var books = CreateBooks(2);
            var ratedBook = books[0];

            var ratingRes = Rating.Create(_userId, ratedBook.Id, 5);
            Assert.That(ratingRes.IsSuccess);
            await _dbContext.Ratings.AddAsync(ratingRes.Value);
            await _dbContext.SaveChangesAsync();

            var result = await _handler.Handle(
                new ExcludeIrrelevantBooksCommand(books, _userId),
                CancellationToken.None);

            Assert.That(result.Select(b => b.Id), Does.Not.Contain(ratedBook.Id));
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task BooksFromStaticCollections_AreExcluded()
        {
            var books = CreateBooks(2);

            // Коллекция создаётся через DTO и фабрику
            var createColDto = new CreateBookCollectionDto("Прочитано", false, _userId);
            var collectionRes = BookCollection.Create(createColDto, isStatic: true);
            Assert.That(collectionRes.IsSuccess);
            var collection = collectionRes.Value;

            // Добавляем книгу в коллекцию
            collection.AddBookAndUpdateCover(books[1]);

            await _dbContext.BookCollections.AddAsync(collection);
            await _dbContext.SaveChangesAsync();

            var result = await _handler.Handle(
                new ExcludeIrrelevantBooksCommand(books, _userId),
                CancellationToken.None);

            Assert.That(result.Select(b => b.Id), Does.Not.Contain(books[1].Id));
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task IrrelevantRecommendations_AreExcluded()
        {
            var books = CreateBooks(2);
            var badBook = books[0];

            var recDto = new RecommendationDto(badBook.Id, RecommendationStatus.Irrelevant);
            var rec = Recommendation.Create(recDto, _userId);
            await _dbContext.Recommendations.AddAsync(rec);
            await _dbContext.SaveChangesAsync();

            var result = await _handler.Handle(
                new ExcludeIrrelevantBooksCommand(books, _userId),
                CancellationToken.None);

            Assert.That(result.Select(b => b.Id), Does.Not.Contain(badBook.Id));
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task BooksWithHatedGenres_AreExcluded()
        {
            var books = CreateBooks(2);

            // Жанр создаётся через DTO и фабрику
            var genreRes = Genre.Create(new CreateGenreDto("Horror", "Ужасы"));
            Assert.That(genreRes.IsSuccess);
            var hatedGenre = genreRes.Value;
            _dbContext.Genres.Add(hatedGenre);

            // К книге добавляем ненавидимый жанр
            books[0].AddGenres(new[] { hatedGenre });

            // Добавляем dislike предпочтение
            var prefDto = new UserPreferenceDto(_userId, hatedGenre.Id, PreferenceType.Disliked, -1);
            var userPref = UserGenrePreference.Create(prefDto);
            await _dbContext.UserGenrePreferences.AddAsync(userPref);
            await _dbContext.SaveChangesAsync();

            var result = await _handler.Handle(
                new ExcludeIrrelevantBooksCommand(books, _userId),
                CancellationToken.None);

            Assert.That(result.Select(b => b.Id), Does.Not.Contain(books[0].Id));
            Assert.That(result.Single().Id, Is.EqualTo(books[1].Id));
        }

        // ---------- HELPERS ----------

        private static List<Book> CreateBooks(int count)
        {
            var list = new List<Book>();
            for (int i = 0; i < count; i++)
            {
                var bookDto = new CreateBookDto(
                    Title: $"Book {i}",
                    Description: null,
                    Rating: 0,
                    RatingsCount: 0,
                    Language: "ru",
                    Publisher: "TestPub",
                    PublishmentYear: 2020,
                    PageCount: 150,
                    AgeRestriction: AgeRestriction.Children, 
                    Thumbnail: null,
                    ExternalId: Guid.NewGuid().ToString(),
                    Authors: Array.Empty<string>(),
                    Genres: Array.Empty<string>()
                );
                var result = Book.Create(bookDto);
                Assert.That(result.IsSuccess);
                typeof(Book).GetProperty("Id").SetValue(result.Value, Guid.NewGuid());
                list.Add(result.Value);
            }
            return list;
        }
    }
}