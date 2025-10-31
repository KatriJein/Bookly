using Bookly.Application.Handlers.Authors;
using Bookly.Application.Handlers.Books;
using Bookly.Application.Handlers.Genres;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core;
using Core.Dto.Author;
using Core.Dto.Book;
using Core.Dto.Genre;
using Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Bookly.Tests.Application.Handlers.Books;

[TestFixture]
    public class CreateBookHandlerTests
    {
        private BooklyDbContext _db = null!;
        private IMediator _mediator = null!;

        [SetUp]
        public void Setup()
        {
            _db = DatabaseUtils.CreateDbContext();
            _mediator = Substitute.For<IMediator>();
        }

        [TearDown]
        public void Teardown()
        {
            _db.Dispose();
        }

        [Test]
        public async Task Creates_NewBook_WhenNotExists()
        {
            _mediator.Send(Arg.Any<GetAuthorQuery>(), Arg.Any<CancellationToken>())
                .Returns((Author?)null);
            _mediator.Send(Arg.Any<CreateAuthorCommand>(), Arg.Any<CancellationToken>())
                .Returns(Result<Author>.Success(Author.Create(new CreateAuthorDto("Толстой Л.Н.", "Лев Толстой")).Value));
            _mediator.Send(Arg.Any<AddNewGenreCommand>(), Arg.Any<CancellationToken>())
                .Returns(Result<Genre>.Success(Genre.Create(new CreateGenreDto("Fiction", "Художка")).Value));

            var dto = new CreateBookDto(
                "Война и мир", "Описание", 4.5, 999, "ru", "Изд-во",
                1869, 1300, AgeRestriction.Everyone, "thumb", "ext1",
                new[] { "Толстой Л.Н." }, new[] { "Fiction" });

            var handler = new CreateBookHandler(_mediator, _db, Substitute.For<Serilog.ILogger>());
            var res = await handler.Handle(new CreateBookCommand(dto), CancellationToken.None);

            Assert.That(res.IsSuccess);
            Assert.That(await _db.Books.CountAsync(), Is.EqualTo(1));
            var saved = await _db.Books.Include(b => b.Authors).Include(b => b.Genres).FirstAsync();
            Assert.That(saved.Authors.First().Name, Contains.Substring("Толстой"));
            Assert.That(saved.Genres.First().Name, Is.EqualTo("Fiction"));
        }

        [Test]
        public async Task Returns_ExistingBook_WhenFoundByExternalId()
        {
            var existing = Book.Create(new CreateBookDto(
                "Старая книга", "Описание", 3, 1, "ru", "Изд", 1990, 100, AgeRestriction.Everyone,
                "thumb", "ext_dup", Array.Empty<string>(), Array.Empty<string>())).Value;
            _db.Books.Add(existing);
            await _db.SaveChangesAsync();

            var dto = new CreateBookDto(
                "Старая книга", "Описание", 4, 100, "ru", "Изд", 1990, 100, AgeRestriction.Everyone,
                "thumb", "ext_dup", Array.Empty<string>(), Array.Empty<string>());

            var handler = new CreateBookHandler(_mediator, _db, Substitute.For<Serilog.ILogger>());
            var res = await handler.Handle(new CreateBookCommand(dto), CancellationToken.None);

            Assert.That(res.IsSuccess);
            Assert.That(res.Value, Is.EqualTo(existing.Id));
            Assert.That(await _db.Books.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task Returns_Failure_WhenInvalidData()
        {
            var dto = new CreateBookDto(
                "", "Описание", -5, -5, "", "Изд", 9999, -1, AgeRestriction.Everyone,
                "thumb", "err1", Array.Empty<string>(), Array.Empty<string>());

            var handler = new CreateBookHandler(_mediator, _db, Substitute.For<Serilog.ILogger>());
            var res = await handler.Handle(new CreateBookCommand(dto), CancellationToken.None);

            Assert.That(res.IsFailure);
            Assert.That(await _db.Books.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public async Task Creates_New_Authors_And_Genres_When_NoneExist()
        {
            _mediator.Send(Arg.Any<GetAuthorQuery>(), Arg.Any<CancellationToken>())
                .Returns((Author?)null);
            _mediator.Send(Arg.Any<CreateAuthorCommand>(), Arg.Any<CancellationToken>())
                .Returns(x =>
                {
                    var cmd = x.Arg<CreateAuthorCommand>();
                    return Result<Author>.Success(Author.Create(cmd.CreateAuthorDto).Value);
                });
            _mediator.Send(Arg.Any<AddNewGenreCommand>(), Arg.Any<CancellationToken>())
                .Returns(x =>
                {
                    var cmd = x.Arg<AddNewGenreCommand>();
                    return Result<Genre>.Success(Genre.Create(cmd.CreateGenreDto).Value);
                });

            var dto = new CreateBookDto(
                "Новая книга", "Desc", 5, 100, "ru", "Изд", 2020, 500,
                AgeRestriction.Everyone, "thumb", "ext2",
                new[] { "Автор1", "Автор2" }, new[] { "G1", "G2" });

            var handler = new CreateBookHandler(_mediator, _db, Substitute.For<Serilog.ILogger>());
            var res = await handler.Handle(new CreateBookCommand(dto), CancellationToken.None);

            Assert.That(res.IsSuccess);
            var saved = await _db.Books.Include(b => b.Authors).Include(b => b.Genres).FirstAsync();
            Assert.That(saved.Authors.Count, Is.EqualTo(2));
            Assert.That(saved.Genres.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Adds_Existing_Authors_And_Genres()
        {
            var author = Author.Create(new CreateAuthorDto("Пушкин А.С.", "Александр Пушкин")).Value;
            var genre = Genre.Create(new CreateGenreDto("Poetry", "Поэзия")).Value;
            _db.Authors.Add(author);
            _db.Genres.Add(genre);
            await _db.SaveChangesAsync();

            _mediator.Send(Arg.Is<GetAuthorQuery>(q => q.Name == "Пушкин А.С."), Arg.Any<CancellationToken>())
                .Returns(author);

            var dto = new CreateBookDto(
                "Онегин", "Описание", 4.8, 999, "ru", "Изд", 1830, 400,
                AgeRestriction.Everyone, "thumb", "ext3",
                new[] { "Пушкин А.С." }, new[] { "Poetry" });

            var handler = new CreateBookHandler(_mediator, _db, Substitute.For<Serilog.ILogger>());
            var res = await handler.Handle(new CreateBookCommand(dto), CancellationToken.None);

            Assert.That(res.IsSuccess);
            var saved = await _db.Books.Include(b => b.Authors).Include(b => b.Genres).FirstAsync();
            Assert.That(saved.Authors.First().Id, Is.EqualTo(author.Id));
            Assert.That(saved.Genres.First().Id, Is.EqualTo(genre.Id));
        }

        [Test]
        public async Task Ignores_Failed_Create_Author_Or_Genre()
        {
            _mediator.Send(Arg.Any<GetAuthorQuery>(), Arg.Any<CancellationToken>())
                .Returns((Author?)null);
            _mediator.Send(Arg.Any<CreateAuthorCommand>(), Arg.Any<CancellationToken>())
                .Returns(Result<Author>.Failure("ошибка"));
            _mediator.Send(Arg.Any<AddNewGenreCommand>(), Arg.Any<CancellationToken>())
                .Returns(Result<Genre>.Failure("ошибка"));

            var dto = new CreateBookDto(
                "Без автора", "desc", 3, 10, "ru", "Изд", 2000, 400, AgeRestriction.Everyone,
                "thumb", "ext4", new[] { "Автор" }, new[] { "Genre" });

            var handler = new CreateBookHandler(_mediator, _db, Substitute.For<Serilog.ILogger>());
            var res = await handler.Handle(new CreateBookCommand(dto), CancellationToken.None);

            Assert.That(res.IsSuccess);
            var book = await _db.Books.Include(b => b.Authors).Include(b => b.Genres).FirstAsync();
            Assert.That(book.Authors, Is.Empty);
            Assert.That(book.Genres, Is.Empty);
        }

        [Test]
        public async Task Trims_Authors_And_Genres_Before_Search()
        {
            var author = Author.Create(new CreateAuthorDto("Гоголь Н.В.", "Николай Гоголь")).Value;
            var genre = Genre.Create(new CreateGenreDto("Novel", "Роман")).Value;
            _db.Authors.Add(author);
            _db.Genres.Add(genre);
            await _db.SaveChangesAsync();

            _mediator.Send(Arg.Is<GetAuthorQuery>(q => q.Name.Trim() == "Гоголь Н.В."), Arg.Any<CancellationToken>())
                .Returns(author);

            var dto = new CreateBookDto(
                "Мертвые души", "desc", 5, 3000, "ru", "Изд", 1841, 300, AgeRestriction.Everyone,
                "thumb", "ext5", new[] { "  Гоголь Н.В. " }, new[] { "  Novel " });

            var handler = new CreateBookHandler(_mediator, _db, Substitute.For<Serilog.ILogger>());
            var res = await handler.Handle(new CreateBookCommand(dto), CancellationToken.None);

            Assert.That(res.IsSuccess);
            Assert.That(await _db.Books.CountAsync(), Is.EqualTo(1));
        }
    }