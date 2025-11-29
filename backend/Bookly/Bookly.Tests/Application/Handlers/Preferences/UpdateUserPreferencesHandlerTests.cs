using Bookly.Application.Handlers.Preferences;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.Author;
using Core.Dto.Book;
using Core.Dto.Genre;
using Core.Dto.Preferences;
using Core.Dto.User;
using Core.Enums;
using Core.Payloads;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Serilog;

namespace Bookly.Tests.Application.Handlers.Preferences
{
    [TestFixture]
    public class UpdateUserPreferencesHandlerTests
    {
        private BooklyDbContext _dbContext = null!;
        private UpdateUserPreferencesHandler _handler = null!;
        private Guid _userId;
        private Author _author = null!;
        private Genre _genre = null!;
        private Book _book = null!;

        [SetUp]
        public async Task SetUp()
        {
            _dbContext = DatabaseUtils.CreateDbContext();
            var logger = new Mock<ILogger>();
            _handler = new UpdateUserPreferencesHandler(_dbContext, logger.Object);

            // ---- создаём пользователя через фабрику ----
            var userRes = User.Create(new CreateUserDto("tester", "tester@example.com", "Secret123"));
            Assert.That(userRes.IsSuccess);
            var user = userRes.Value;
            _userId = user.Id;
            await _dbContext.Users.AddAsync(user);

            // ---- создаём автора ----
            _author = Author.Create(new CreateAuthorDto("Asimov", "Айзек Азимов")).Value;
            await _dbContext.Authors.AddAsync(_author);

            // ---- создаём жанр ----
            _genre = Genre.Create(new CreateGenreDto("sci-fi", "Научная фантастика")).Value;
            await _dbContext.Genres.AddAsync(_genre);

            // ---- создаём книгу ----
            var bookDto = new CreateBookDto(
                Title: "Foundation",
                Description: "Classic",
                Rating: 0,
                RatingsCount: 0,
                Language: "en",
                Publisher: "Pub",
                PublishmentYear: 1951,
                PageCount: 500,
                AgeRestriction: AgeRestriction.Children,
                Thumbnail: null,
                ExternalId: Guid.NewGuid().ToString(),
                Authors: new[] { _author.Name },
                Genres: new[] { _genre.Name });
            _book = Book.Create(bookDto).Value;
            _book.AddAuthors(new[] { _author });
            _book.AddGenres(new[] { _genre });
            await _dbContext.Books.AddAsync(_book);

            await _dbContext.SaveChangesAsync();
        }

        [TearDown]
        public void TearDown() => _dbContext.Dispose();

        [Test]
        public async Task AddedToFavourites_AddsPositivePreferences()
        {
            var payload = new AddedToFavouritesPreferenceActionPayload();
            var dto = new PreferencePayloadDto(_book.Id, _userId, payload);
            var cmd = new UpdateUserPreferencesCommand(dto);

            await _handler.Handle(cmd, CancellationToken.None);
            await _dbContext.SaveChangesAsync();

            var authorPref = await _dbContext.UserAuthorPreferences.FirstAsync(p => p.UserId == _userId);
            var genrePref = await _dbContext.UserGenrePreferences.FirstAsync(p => p.UserId == _userId);

            Assert.That(authorPref.Weight, Is.GreaterThan(0));
            Assert.That(genrePref.Weight, Is.GreaterThan(0));
        }

        [Test]
        public async Task RemovedFromFavourites_DecreasesPreferenceWeights()
        {
            await AddedToFavourites_AddsPositivePreferences();

            var payload = new RemovedFromFavouritesPreferenceActionPayload();
            var dto = new PreferencePayloadDto(_book.Id, _userId, payload);
            var cmd = new UpdateUserPreferencesCommand(dto);

            await _handler.Handle(cmd, CancellationToken.None);
            await _dbContext.SaveChangesAsync();

            var authorPref = await _dbContext.UserAuthorPreferences.FirstAsync(p => p.UserId == _userId);
            var genrePref = await _dbContext.UserGenrePreferences.FirstAsync(p => p.UserId == _userId);

            Assert.That(authorPref.Weight, Is.LessThanOrEqualTo(0.35));
            Assert.That(genrePref.Weight, Is.LessThanOrEqualTo(0.35));
        }

        [Test]
        public async Task RatedBook_UpdatesWeightWithRatingValue()
        {
            var payload = new RatedPreferenceActionPayload(5);
            var dto = new PreferencePayloadDto(_book.Id, _userId, payload);
            var cmd = new UpdateUserPreferencesCommand(dto);
            await _handler.Handle(cmd, CancellationToken.None);
            await _dbContext.SaveChangesAsync();

            var genrePref = await _dbContext.UserGenrePreferences.FirstAsync(p => p.UserId == _userId);
            Assert.That(genrePref.Weight, Is.InRange(0.05, 1.0));
        }

        [Test]
        public async Task ReadBook_AddsPositiveDelta()
        {
            var payload = new ReadBookPreferenceActionPayload();
            var dto = new PreferencePayloadDto(_book.Id, _userId, payload);
            var cmd = new UpdateUserPreferencesCommand(dto);
            await _handler.Handle(cmd, CancellationToken.None);
            await _dbContext.SaveChangesAsync();

            var genrePref = await _dbContext.UserGenrePreferences.FirstAsync(p => p.UserId == _userId);
            Assert.That(genrePref.Weight, Is.GreaterThan(0.0));
        }

        [Test]
        public async Task UnknownAction_DoesNotAffectPreferences()
        {
            var payload = new DummyPreferenceActionPayload();
            var dto = new PreferencePayloadDto(_book.Id, _userId, payload);
            var cmd = new UpdateUserPreferencesCommand(dto);
            await _handler.Handle(cmd, CancellationToken.None);
            await _dbContext.SaveChangesAsync();

            Assert.Multiple(() =>
            {
                Assert.That(_dbContext.UserAuthorPreferences.Any(p => p.UserId == _userId), Is.False);
                Assert.That(_dbContext.UserGenrePreferences.Any(p => p.UserId == _userId), Is.False);
            });
        }
        
        [Test]
        public async Task SecondInteraction_UpdatesExistingPreferences_NotCreatesNewOnes()
        {
            var addPayload = new AddedToFavouritesPreferenceActionPayload();
            var addDto = new PreferencePayloadDto(_book.Id, _userId, addPayload);
            var addCmd = new UpdateUserPreferencesCommand(addDto);
            await _handler.Handle(addCmd, CancellationToken.None);
            await _dbContext.SaveChangesAsync();
            
            var initialAuthorPrefsCount = await _dbContext.UserAuthorPreferences.CountAsync();
            var initialGenrePrefsCount  = await _dbContext.UserGenrePreferences.CountAsync();
            var initialAuthorWeight = (await _dbContext.UserAuthorPreferences.FirstAsync()).Weight;
            var initialGenreWeight  = (await _dbContext.UserGenrePreferences.FirstAsync()).Weight;
            
            await _handler.Handle(addCmd, CancellationToken.None);
            await _dbContext.SaveChangesAsync();
            
            var authorPrefsCountAfter = await _dbContext.UserAuthorPreferences.CountAsync();
            var genrePrefsCountAfter  = await _dbContext.UserGenrePreferences.CountAsync();
            var authorWeightAfter     = (await _dbContext.UserAuthorPreferences.FirstAsync()).Weight;
            var genreWeightAfter      = (await _dbContext.UserGenrePreferences.FirstAsync()).Weight;
            
            Assert.That(authorPrefsCountAfter, Is.EqualTo(initialAuthorPrefsCount));
            Assert.That(genrePrefsCountAfter,  Is.EqualTo(initialGenrePrefsCount));
            
            Assert.That(authorWeightAfter, Is.GreaterThan(initialAuthorWeight));
            Assert.That(genreWeightAfter,  Is.GreaterThan(initialGenreWeight));
        }
        
        [Test]
        public async Task StartedToReadBook_AddsSmallerPositiveDelta()
        {
            
            var payload = new StartedToReadBookPreferenceActionPayload();
            var dto = new PreferencePayloadDto(_book.Id, _userId, payload);
            var cmd = new UpdateUserPreferencesCommand(dto);
            
            await _handler.Handle(cmd, CancellationToken.None);
            await _dbContext.SaveChangesAsync();
            var authorPref = await _dbContext.UserAuthorPreferences.FirstAsync(p => p.UserId == _userId);
            var genrePref  = await _dbContext.UserGenrePreferences.FirstAsync(p => p.UserId == _userId);

            Assert.That(authorPref.Weight, Is.InRange(0.01, 0.4));
            Assert.That(genrePref.Weight,  Is.InRange(0.01, 0.4));
        }
        
        [Test]
        public async Task NonExistingUser_NoChanges()
        {
            var nonexistentUserId = Guid.NewGuid();
            var payload = new AddedToFavouritesPreferenceActionPayload();
            var dto = new PreferencePayloadDto(_book.Id, nonexistentUserId, payload);
            var cmd = new UpdateUserPreferencesCommand(dto);

            await _handler.Handle(cmd, CancellationToken.None);
            await _dbContext.SaveChangesAsync();

            Assert.Multiple(() =>
            {
                Assert.That(_dbContext.UserAuthorPreferences.Any(), Is.False);
                Assert.That(_dbContext.UserGenrePreferences.Any(), Is.False);
            });
        }

        [Test]
        public async Task NonExistingBook_NoChanges()
        {
            var payload = new AddedToFavouritesPreferenceActionPayload();
            var dto = new PreferencePayloadDto(Guid.NewGuid(), _userId, payload);
            var cmd = new UpdateUserPreferencesCommand(dto);

            await _handler.Handle(cmd, CancellationToken.None);
            await _dbContext.SaveChangesAsync();

            Assert.Multiple(() =>
            {
                Assert.That(_dbContext.UserAuthorPreferences.Any(), Is.False);
                Assert.That(_dbContext.UserGenrePreferences.Any(), Is.False);
            });
        }
        
        private class DummyPreferenceActionPayload() : IPrerefenceActionPayload;
    }
}