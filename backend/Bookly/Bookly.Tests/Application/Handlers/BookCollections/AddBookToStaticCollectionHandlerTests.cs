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
public class AddBookToStaticCollectionHandlerTests
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
    public async Task Handle_AddsBook_WhenStaticCollectionExistsForUser()
    {
        var userDto = new CreateUserDto("User", "u@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var bookDto = new CreateBookDto("Книга", "desc", 5, 100, "ru", "Publisher", 2020, 200, AgeRestriction.Everyone, null, "k1", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        var collection = BookCollection.Create(new CreateBookCollectionDto("Избранное", true, user.Id), true).Value;
        _db.BookCollections.Add(collection);
        await _db.SaveChangesAsync();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<AddBookToBookCollectionsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        var handler = new AddBookToStaticCollectionHandler(_mediatorMock.Object, _db);

        var command = new AddBookToStaticCollectionCommand("Избранное", book.Id, user.Id);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsSuccess);
        _mediatorMock.Verify(m => m.Send(It.Is<AddBookToBookCollectionsCommand>(
            c => c.CollectionIds.Single() == collection.Id &&
                 c.BookId == book.Id &&
                 c.UserId == user.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenCollectionDoesNotExistOrNotOwned()
    {
        var userDto1 = new CreateUserDto("Owner", "o@mail.com", "hash");
        var userDto2 = new CreateUserDto("Intruder", "i@mail.com", "hash");
        var owner = User.Create(userDto1).Value;
        var intruder = User.Create(userDto2).Value;
        _db.Users.AddRange(owner, intruder);
        await _db.SaveChangesAsync();

        var bookDto = new CreateBookDto("Book", "desc", 4, 100, "ru", "Publisher", 2020, 200, AgeRestriction.Everyone, null, "k2", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        var ownerCollection = BookCollection.Create(new CreateBookCollectionDto("Избранное", true, owner.Id), true).Value;
        _db.BookCollections.Add(ownerCollection);
        await _db.SaveChangesAsync();

        var handler = new AddBookToStaticCollectionHandler(_mediatorMock.Object, _db);

        var command = new AddBookToStaticCollectionCommand("Избранное", book.Id, intruder.Id);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Несуществующая коллекция или не принадлежащая пользователю"));
        _mediatorMock.Verify(m => m.Send(It.IsAny<IRequest<Result>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}