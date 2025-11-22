using Bookly.Application.Handlers.BookCollections;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core;
using Core.Dto.Book;
using Core.Dto.BookCollection;
using Core.Dto.User;
using Core.Enums;
using MediatR;
using Moq;

namespace Bookly.Tests.Application.Handlers.BookCollections;

[TestFixture]
public class RemoveBookFromStaticCollectionHandlerTests
{
    private BooklyDbContext _db = null!;
    private Mock<IMediator> _mediatorMock = null!;

    [SetUp]
    public void Setup()
    {
        _db = DatabaseUtils.CreateDbContext();
        _mediatorMock = new Mock<IMediator>();
    }

    [TearDown]
    public void Teardown()
    {
        _db.Dispose();
    }

    [Test]
    public async Task Handle_RemovesBook_WhenCollectionExistsForUser()
    {
        var userDto = new CreateUserDto("User", "u@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);

        var bookDto = new CreateBookDto("Книга", "desc", 4, 100, "ru", "Pub", 2020, 200, AgeRestriction.Everyone, null, "k1",
            Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);

        var staticCollection = BookCollection.Create(new CreateBookCollectionDto("Прочитано", true, user.Id), true).Value;
        _db.BookCollections.Add(staticCollection);
        await _db.SaveChangesAsync();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<RemoveBookFromBookCollectionCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var handler = new RemoveBookFromStaticCollectionHandler(_mediatorMock.Object, _db);
        var command = new RemoveBookFromStaticCollectionCommand("Прочитано", book.Id, user.Id);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsSuccess);
        _mediatorMock.Verify(m => m.Send(It.Is<RemoveBookFromBookCollectionCommand>(
            c => c.BookId == book.Id &&
                 c.CollectionId == staticCollection.Id &&
                 c.UserId == user.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenCollectionNotFoundOrBelongsToAnotherUser()
    {
        var ownerDto = new CreateUserDto("Owner", "o@mail.com", "hash");
        var otherDto = new CreateUserDto("Other", "x@mail.com", "hash");
        var owner = User.Create(ownerDto).Value;
        var other = User.Create(otherDto).Value;
        _db.Users.AddRange(owner, other);

        var bookDto = new CreateBookDto("Book", "d", 4, 100, "ru", "Pub", 2020, 200, AgeRestriction.Everyone, null, "k2",
            Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);

        var staticCollection = BookCollection.Create(new CreateBookCollectionDto("Хочу прочитать", true, owner.Id), true).Value;
        _db.BookCollections.Add(staticCollection);
        await _db.SaveChangesAsync();

        var handler = new RemoveBookFromStaticCollectionHandler(_mediatorMock.Object, _db);
        var command = new RemoveBookFromStaticCollectionCommand("Хочу прочитать", book.Id, other.Id);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Несуществующая коллекция или не принадлежащая пользователю"));
        _mediatorMock.Verify(m => m.Send(It.IsAny<RemoveBookFromBookCollectionCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}