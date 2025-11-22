using Bookly.Application.Handlers.Reviews;
using Bookly.Domain;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.Book;
using Core.Dto.Review;
using Core.Dto.User;
using Core.Enums;

namespace Bookly.Tests.Application.Handlers.Reviews;

[TestFixture]
public class UpdateReviewHandlerTests
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
    public async Task Handle_UpdatesReview_WhenOwnedByUser_AndTextValid()
    {
        var userDto = new CreateUserDto("User", "u@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);

        var bookDto = new CreateBookDto("Book", "desc", 0, 0, "ru", "Pub", 2020, 200,
            AgeRestriction.Everyone, null, "ext", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);

        var reviewDto = new CreateReviewDto("Очень интересная книга!", book.Id);
        var review = Review.Create(reviewDto, user.Id).Value;
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        var updateDto = new UpdateReviewDto("Обновлённый, более развёрнутый отзыв о книге.");
        var cmd = new UpdateReviewCommand(updateDto, review.Id, user.Id);
        var handler = new UpdateReviewHandler(_db);

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.That(result.IsSuccess);
        var updated = _db.Reviews.First();
        Assert.That(updated.Text, Is.EqualTo(updateDto.Text));
        Assert.That(updated.UpdatedAt, Is.GreaterThan(updated.CreatedAt));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenReviewNotFound()
    {
        var userDto = new CreateUserDto("User", "nf@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var updateDto = new UpdateReviewDto("Текст неважен");
        var cmd = new UpdateReviewCommand(updateDto, Guid.NewGuid(), user.Id);
        var handler = new UpdateReviewHandler(_db);

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Несуществующий или не принадлежащий пользователю отзыв"));
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenTextTooShort()
    {
        var userDto = new CreateUserDto("User", "short@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);

        var bookDto = new CreateBookDto("Book", "desc", 0, 0, "ru", "Pub", 2020, 200,
            AgeRestriction.Everyone, null, "ext2", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);

        var reviewDto = new CreateReviewDto("Длинный отзыв", book.Id);
        var review = Review.Create(reviewDto, user.Id).Value;
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        var updateDto = new UpdateReviewDto("корот");
        var cmd = new UpdateReviewCommand(updateDto, review.Id, user.Id);
        var handler = new UpdateReviewHandler(_db);

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Пустой отзыв или слишком маленькая длина отзыва"));
        Assert.That(_db.Reviews.First().Text, Is.EqualTo("Длинный отзыв"));
    }

    [Test]
    public async Task Handle_DoesNothing_WhenReviewBelongsToAnotherUser()
    {
        var ownerDto = new CreateUserDto("Owner", "o@mail.com", "hash");
        var strangerDto = new CreateUserDto("Stranger", "s@mail.com", "hash");
        var owner = User.Create(ownerDto).Value;
        var stranger = User.Create(strangerDto).Value;
        _db.Users.AddRange(owner, stranger);

        var bookDto = new CreateBookDto("Book", "desc", 0, 0, "ru", "Pub", 2020, 200,
            AgeRestriction.Everyone, null, "ext3", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);

        var reviewDto = new CreateReviewDto("Отличный роман!", book.Id);
        var review = Review.Create(reviewDto, owner.Id).Value;
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        var updateDto = new UpdateReviewDto("Попытка изменить чужой отзыв.");
        var cmd = new UpdateReviewCommand(updateDto, review.Id, stranger.Id);
        var handler = new UpdateReviewHandler(_db);

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.That(result.IsFailure);
        Assert.That(result.Error, Is.EqualTo("Несуществующий или не принадлежащий пользователю отзыв"));
        Assert.That(_db.Reviews.First().Text, Is.EqualTo("Отличный роман!"));
    }
}