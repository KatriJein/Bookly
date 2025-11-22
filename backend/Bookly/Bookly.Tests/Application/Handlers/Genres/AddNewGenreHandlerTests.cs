using Bookly.Application.Handlers.Genres;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.Genre;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Bookly.Tests.Application.Handlers.Genres;

[TestFixture]
    public class AddNewGenreHandlerTests
    {
        private DbContextOptions<BooklyDbContext> _options = null!;
        private BooklyDbContext _dbContext = null!;
        private Mock<Serilog.ILogger> _loggerMock = null!;

        [SetUp]
        public void Setup()
        {
            _dbContext = DatabaseUtils.CreateDbContext();
            _loggerMock = new Mock<Serilog.ILogger>();
        }

        [TearDown]
        public void Teardown() => _dbContext.Dispose();

        [Test]
        public async Task Handle_AddsNewGenre_WhenNotExists()
        {
            var dto = new CreateGenreDto("Fantasy", "Фэнтези");
            var command = new AddNewGenreCommand(dto);
            var handler = new AddNewGenreHandler(_dbContext, _loggerMock.Object);
            
            var result = await handler.Handle(command, CancellationToken.None);
            
            Assert.That(result.IsSuccess);
            Assert.That(result.Value.Name, Is.EqualTo("Fantasy"));
            Assert.That(await _dbContext.Genres.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task Handle_ReturnsExistingGenre_WhenAlreadyExists()
        {
            var existing = Genre.Create(new CreateGenreDto("Fantasy", "Фэнтези")).Value;
            _dbContext.Genres.Add(existing);
            await _dbContext.SaveChangesAsync();

            var dto = new CreateGenreDto("Fantasy", "Фэнтези");
            var command = new AddNewGenreCommand(dto);
            var handler = new AddNewGenreHandler(_dbContext, _loggerMock.Object);
            
            var result = await handler.Handle(command, CancellationToken.None);
            
            Assert.That(result.IsSuccess);
            Assert.That(result.Value.Id, Is.EqualTo(existing.Id));
            Assert.That(await _dbContext.Genres.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task Handle_ReturnsFailure_WhenFactoryFails()
        {
            var dto = new CreateGenreDto("", "Описание");
            var command = new AddNewGenreCommand(dto);
            var handler = new AddNewGenreHandler(_dbContext, _loggerMock.Object);
            
            var result = await handler.Handle(command, CancellationToken.None);
            
            Assert.That(result.IsFailure);
            Assert.That(result.Error, Is.Not.Empty);
            Assert.That(await _dbContext.Genres.CountAsync(), Is.EqualTo(0));
        }
    }