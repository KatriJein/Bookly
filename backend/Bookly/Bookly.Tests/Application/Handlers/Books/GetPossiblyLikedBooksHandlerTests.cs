using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bookly.Application.Handlers.Books;
using Bookly.Application.Handlers.Rateable;
using Bookly.Application.Handlers.Recommendations;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Data;
using Core.Dto.Book;
using Core.Dto.BookCollection;
using Core.Dto.Genre;
using Core.Dto.Preferences;
using Core.Dto.User;
using Core.Dto.UserGenrePreference;
using Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Bookly.Tests.Application.Handlers.Books
{
    [TestFixture]
    public class GetPossiblyLikedBooksHandlerTests
    {
        private BooklyDbContext _dbContext = null!;
        private Mock<IMediator> _mediatorMock = null!;
        private GetPossiblyLikedBooksHandler _handler = null!;
        private Guid _userId;
        private Genre _genre1 = null!;
        private Genre _genre2 = null!;
        private Book _book1 = null!;
        private Book _book2 = null!;

        [SetUp]
        public async Task SetUp()
        {
            _dbContext = DatabaseUtils.CreateDbContext();

            // Заглушка IMediator
            _mediatorMock = new Mock<IMediator>();
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ExcludeIrrelevantBooksAndEnrichRelevantWithDataCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((ExcludeIrrelevantBooksAndEnrichRelevantWithDataCommand cmd, CancellationToken _) => cmd.Books);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CalculateAverageRatingQuery<Book>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(3.5);

            _handler = new GetPossiblyLikedBooksHandler(_mediatorMock.Object, _dbContext);

            // --- пользователь ---
            var userRes = User.Create(new CreateUserDto("tester", "t@t.com", "Hash"));
            Assert.That(userRes.IsSuccess);
            var user = userRes.Value;
            _userId = user.Id;
            await _dbContext.Users.AddAsync(user);

            // --- жанры ---
            _genre1 = Genre.Create(new CreateGenreDto("sci-fi", "Science Fiction")).Value;
            _genre2 = Genre.Create(new CreateGenreDto("fantasy", "Fantasy")).Value;
            await _dbContext.Genres.AddRangeAsync(_genre1, _genre2);

            // --- книги ---
            _book1 = Book.Create(new CreateBookDto(
                "Book1", "desc", 5, 100, "en", "Pub", 2000,
                400, AgeRestriction.Children, null, Guid.NewGuid().ToString(),
                Array.Empty<string>(), new[] { "sci-fi" }
            )).Value;

            _book2 = Book.Create(new CreateBookDto(
                "Book2", "desc", 4.8, 80, "en", "Pub", 2001,
                350, AgeRestriction.Children, null, Guid.NewGuid().ToString(),
                Array.Empty<string>(), new[] { "fantasy" }
            )).Value;

            _book1.AddGenres(new[] { _genre1 });
            _book2.AddGenres(new[] { _genre2 });

            await _dbContext.Books.AddRangeAsync(_book1, _book2);
            await _dbContext.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown() => _dbContext.Dispose();

        // ---------------- NEGATIVE TESTS ----------------

        [Test]
        public async Task Handle_WhenUserIdIsNull_ReturnsFailure()
        {
            var query = new GetPossiblyLikedBooksQuery(new BookSimpleSearchSettingsDto(1, 10), null);
            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.That(result.IsFailure);
            Assert.That(result.Error, Does.Contain("только авторизованным"));
        }

        [Test]
        public async Task Handle_WhenUserDoesNotExist_ReturnsFailure()
        {
            var query = new GetPossiblyLikedBooksQuery(new BookSimpleSearchSettingsDto(1, 10), Guid.NewGuid());
            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.That(result.IsFailure);
        }

        // --------------- POSITIVE TESTS ----------------

        [Test]
        public async Task Handle_ReturnsBestBooksOfPreferredGenres()
        {
            // добавляем предпочтение пользователю (любимый жанр sci-fi)
            var pref = UserGenrePreference.Create(
                new UserPreferenceDto(_userId, _genre1.Id, PreferenceType.Liked, 1));
            _dbContext.UserGenrePreferences.Add(pref);
            await _dbContext.SaveChangesAsync();

            var query = new GetPossiblyLikedBooksQuery(new BookSimpleSearchSettingsDto(1, 10), _userId);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.That(result.IsSuccess);
            Assert.That(result.Value, Is.Not.Empty);
            Assert.That(result.Value.Select(b => b.Title), Does.Contain(_book1.Title));
        }

        [Test]
        public async Task Handle_FallbacksToAllGenres_WhenFewPreferences()
        {
            // нет предпочтений
            var query = new GetPossiblyLikedBooksQuery(new BookSimpleSearchSettingsDto(1, 10), _userId);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.That(result.IsSuccess);
            Assert.That(result.Value.Select(b => b.Title),
                Has.Member(_book1.Title).And.Member(_book2.Title));
        }

        [Test]
        public async Task Handle_RespectsGenreWeights()
        {
            // два жанра с разными весами
            var sciPref = UserGenrePreference.Create(
                new UserPreferenceDto(_userId, _genre1.Id, PreferenceType.Liked, 1));
            var fantPref = UserGenrePreference.Create(
                new UserPreferenceDto(_userId, _genre2.Id, PreferenceType.Neutral, 0));

            _dbContext.UserGenrePreferences.AddRange(sciPref, fantPref);
            await _dbContext.SaveChangesAsync();

            var query = new GetPossiblyLikedBooksQuery(new BookSimpleSearchSettingsDto(1, 20), _userId);
            var result = await _handler.Handle(query, CancellationToken.None);

            var books = result.Value;
            var sciCount   = books.Count(b => b.Genres.Any(g => g.Id == _genre1.Id));
            var fantCount  = books.Count(b => b.Genres.Any(g => g.Id == _genre2.Id));

            Assert.That(sciCount, Is.GreaterThanOrEqualTo(fantCount));
        }

        [Test]
        public async Task Handle_ReturnsUniqueBooks_WhenBookBelongsToMultipleGenres()
        {
            var hybrid = Book.Create(new CreateBookDto(
                "Hybrid", "desc", 5, 100, "en", "Pub", 2010,
                500, AgeRestriction.Children, null, Guid.NewGuid().ToString(),
                Array.Empty<string>(), new[] { "sci-fi", "fantasy" }
            )).Value;
            hybrid.AddGenres(new[] { _genre1, _genre2 });

            await _dbContext.Books.AddAsync(hybrid);
            _dbContext.UserGenrePreferences.AddRange(
                UserGenrePreference.Create(new UserPreferenceDto(_userId, _genre1.Id, PreferenceType.Liked, 1)),
                UserGenrePreference.Create(new UserPreferenceDto(_userId, _genre2.Id, PreferenceType.Liked, 1))
            );
            await _dbContext.SaveChangesAsync();

            var query = new GetPossiblyLikedBooksQuery(new BookSimpleSearchSettingsDto(1, 10), _userId);
            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.That(result.IsSuccess);
            Assert.That(result.Value.Count(b => b.Title == "Hybrid"), Is.EqualTo(1));
        }
        
        [Test]
        public async Task Handle_WhenSimilarUserExists_ReturnsBooksFromThatUser()
        {
            var genre3 = Genre.Create(new CreateGenreDto("history", "History")).Value;
            await _dbContext.Genres.AddAsync(genre3);
            await _dbContext.SaveChangesAsync();

            // текущий пользователь: три нейтральных/любимых жанра
            var baseUser = User.Create(new CreateUserDto("base", "b@b.com", "h")).Value;
            await _dbContext.Users.AddAsync(baseUser);
            _dbContext.UserGenrePreferences.AddRange(
                UserGenrePreference.Create(new UserPreferenceDto(baseUser.Id, _genre1.Id, PreferenceType.Liked, 0.8)),
                UserGenrePreference.Create(new UserPreferenceDto(baseUser.Id, _genre2.Id, PreferenceType.Neutral, 0)),
                UserGenrePreference.Create(new UserPreferenceDto(baseUser.Id, genre3.Id, PreferenceType.Neutral, 0.1))
            );

            // похожий пользователь совпадает по sci-fi
            var simUser = User.Create(new CreateUserDto("sim", "s@s.com", "h")).Value;
            await _dbContext.Users.AddAsync(simUser);
            _dbContext.UserGenrePreferences.AddRange(
                UserGenrePreference.Create(new UserPreferenceDto(simUser.Id, _genre1.Id, PreferenceType.Liked, 1))
            );

            // непохожий пользователь, другой жанр
            var diffUser = User.Create(new CreateUserDto("diff", "d@d.com", "h")).Value;
            await _dbContext.Users.AddAsync(diffUser);
            _dbContext.UserGenrePreferences.Add(
                UserGenrePreference.Create(new UserPreferenceDto(diffUser.Id, genre3.Id, PreferenceType.Disliked, -0.8))
            );

            await _dbContext.SaveChangesAsync();

            // похожий пользователь оценил sci-fi книгу
            await _dbContext.Ratings.AddAsync(Rating.Create(simUser.Id, _book1.Id, 5).Value);
            await _dbContext.SaveChangesAsync();

            var query = new GetPossiblyLikedBooksQuery(new BookSimpleSearchSettingsDto(1, 10), baseUser.Id);
            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.That(result.IsSuccess);
            Assert.That(result.Value.Any(b => b.Title == _book1.Title));
        }

        [Test]
        public async Task Handle_WhenNoSimilarUsers_FallsBackToGenreBased()
        {
            var user = User.Create(new CreateUserDto("alone", "a@a.com", "h")).Value;
            await _dbContext.Users.AddAsync(user);
            _dbContext.UserGenrePreferences.AddRange(
                UserGenrePreference.Create(new UserPreferenceDto(user.Id, _genre1.Id, PreferenceType.Liked, 0.6)),
                UserGenrePreference.Create(new UserPreferenceDto(user.Id, _genre2.Id, PreferenceType.Neutral, 0)),
                UserGenrePreference.Create(new UserPreferenceDto(user.Id, _genre2.Id, PreferenceType.Neutral, 0.1))
            );
            await _dbContext.SaveChangesAsync();

            var query = new GetPossiblyLikedBooksQuery(new BookSimpleSearchSettingsDto(1, 10), user.Id);
            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.That(result.IsSuccess);
            Assert.That(result.Value.Any());
        }

        [Test]
        public async Task Handle_DoesNotTriggerSimilarity_WhenLessThanThreePreferences()
        {
            var user = User.Create(new CreateUserDto("two", "t@t.com", "h")).Value;
            await _dbContext.Users.AddAsync(user);
            _dbContext.UserGenrePreferences.AddRange(
                UserGenrePreference.Create(new UserPreferenceDto(user.Id, _genre1.Id, PreferenceType.Liked, 0.8)),
                UserGenrePreference.Create(new UserPreferenceDto(user.Id, _genre2.Id, PreferenceType.Liked, 0.8))
            );
            await _dbContext.SaveChangesAsync();

            var sim = User.Create(new CreateUserDto("simtoo", "s@s.com", "h")).Value;
            await _dbContext.Users.AddAsync(sim);
            _dbContext.UserGenrePreferences.Add(
                UserGenrePreference.Create(new UserPreferenceDto(sim.Id, _genre1.Id, PreferenceType.Liked, 1))
            );
            await _dbContext.SaveChangesAsync();

            await _dbContext.Ratings.AddAsync(Rating.Create(sim.Id, _book1.Id, 5).Value);
            await _dbContext.SaveChangesAsync();

            var query = new GetPossiblyLikedBooksQuery(new BookSimpleSearchSettingsDto(1, 10), user.Id);
            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.That(result.IsSuccess);
            Assert.That(result.Value.Any());
        }
    }
}