using Bookly.Application.Handlers.Authors;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.Author;

namespace Bookly.Tests.Application.Handlers.Authors;

public class GetAuthorHandlerTests
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
        public async Task Handle_ReturnsAuthor_WhenExactMatch()
        {
            var author = Author.Create(new CreateAuthorDto("Пушкин А.С.", "Александр Сергеевич Пушкин")).Value;
            _db.Authors.Add(author);
            await _db.SaveChangesAsync();

            var handler = new GetAuthorHandler(_db);
            var query = new GetAuthorQuery("Пушкин А.С.");

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Пушкин А.С."));
        }

        [Test]
        public async Task Handle_ReturnsAuthor_WhenNameInDifferentFormat_ThreeWords()
        {
            var stored = Author.Create(new CreateAuthorDto("Пушкин А.С.", "Александр Сергеевич Пушкин")).Value;
            _db.Authors.Add(stored);
            await _db.SaveChangesAsync();

            var handler = new GetAuthorHandler(_db);
            var query = new GetAuthorQuery("Александр Сергеевич Пушкин");

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Пушкин А.С."));
        }

        [Test]
        public async Task Handle_ReturnsAuthor_WhenNameInDifferentFormat_TwoWords()
        {
            var stored = Author.Create(new CreateAuthorDto("Пушкин А.", "Александр Пушкин")).Value;
            _db.Authors.Add(stored);
            await _db.SaveChangesAsync();

            var handler = new GetAuthorHandler(_db);
            var query = new GetAuthorQuery("Александр Пушкин");

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Пушкин А."));
        }

        [Test]
        public async Task Handle_ReturnsNull_WhenAuthorNotFound()
        {
            var handler = new GetAuthorHandler(_db);
            var query = new GetAuthorQuery("Несуществующий Автор");

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result, Is.Null);
        }
}