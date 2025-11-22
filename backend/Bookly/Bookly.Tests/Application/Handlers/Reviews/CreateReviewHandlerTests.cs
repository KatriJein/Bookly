using Bookly.Application.Handlers.Reviews;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.Book;
using Core.Dto.Review;
using Core.Dto.User;
using Core.Enums;

namespace Bookly.Tests.Application.Handlers.Reviews;

[TestFixture]
public class CreateReviewHandlerTests
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
    public async Task Handle_CreatesReview_ForExistingBook()
    {
        var userDto = new CreateUserDto("User", "u@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);

        var createBookDto = new CreateBookDto("Book", "desc", 0, 0, "ru", "Pub", 2020, 200,
            AgeRestriction.Everyone, null, "ext", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(createBookDto).Value;
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        var dto = new CreateReviewDto("Хорошая книга, рекомендую!", book.Id);
        var command = new CreateReviewCommand(dto, user.Id);
        var handler = new CreateReviewHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(result.Value, Is.Not.EqualTo(Guid.Empty));

        var created = _db.Reviews.First();
        Assert.That(created.Text, Is.EqualTo("Хорошая книга, рекомендую!"));
        Assert.That(created.BookId, Is.EqualTo(book.Id));
        Assert.That(created.UserId, Is.EqualTo(user.Id));
        Assert.That(created.CreatedAt, Is.Not.EqualTo(default(DateTime)));
        Assert.That(created.UpdatedAt, Is.Not.EqualTo(default(DateTime)));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenBookDoesNotExist()
    {
        var userDto = new CreateUserDto("User", "x@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var dto = new CreateReviewDto("Неплохая книга", Guid.NewGuid());
        var command = new CreateReviewCommand(dto, user.Id);
        var handler = new CreateReviewHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Несуществующая книга"));
        Assert.That(_db.Reviews.Any(), Is.False);
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenTextTooShort()
    {
        var userDto = new CreateUserDto("User", "s@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);

        var createBookDto = new CreateBookDto("Book", "d", 0, 0, "ru", "Pub", 2020, 200,
            AgeRestriction.Everyone, null, "ext2", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(createBookDto).Value;
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        var dto = new CreateReviewDto("Корот.", book.Id);
        var command = new CreateReviewCommand(dto, user.Id);
        var handler = new CreateReviewHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Пустой отзыв или слишком маленькая длина отзыва"));
        Assert.That(_db.Reviews.Any(), Is.False);
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenTextTooLong()
    {
        var userDto = new CreateUserDto("User", "l@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);

        var createBookDto = new CreateBookDto("Book", "d", 0, 0, "ru", "Pub", 2020, 200,
            AgeRestriction.Everyone, null, "ext3", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(createBookDto).Value;
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        var longText = new string('a', 1100);
        var dto = new CreateReviewDto(longText, book.Id);
        var command = new CreateReviewCommand(dto, user.Id);
        var handler = new CreateReviewHandler(_db);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Превышена допустимая длина отзыва"));
        Assert.That(_db.Reviews.Any(), Is.False);
    }
}