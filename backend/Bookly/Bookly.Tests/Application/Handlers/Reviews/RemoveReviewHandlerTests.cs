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
public class RemoveReviewHandlerTests
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
    public async Task Handle_RemovesReview_WhenOwnedByUser()
    {
        var userDto = new CreateUserDto("User", "u@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);

        var bookDto = new CreateBookDto("Book", "d", 0, 0, "ru", "Pub", 2020, 200,
            AgeRestriction.Everyone, null, "ext", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);

        var reviewDto = new CreateReviewDto("Отличная книга, очень понравилась!", book.Id);
        var review = Review.Create(reviewDto, user.Id).Value;
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        var handler = new RemoveReviewHandler(_db);
        var cmd = new RemoveReviewCommand(review.Id, user.Id);

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(_db.Reviews.Any(), Is.False);
    }

    [Test]
    public async Task Handle_DoesNothing_WhenReviewNotFound()
    {
        var userDto = new CreateUserDto("UserNF", "nf@mail.com", "hash");
        var user = User.Create(userDto).Value;
        _db.Users.Add(user);

        var handler = new RemoveReviewHandler(_db);
        var cmd = new RemoveReviewCommand(Guid.NewGuid(), user.Id);

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(_db.Reviews.Any(), Is.False);
    }

    [Test]
    public async Task Handle_DoesNothing_WhenReviewBelongsToAnotherUser()
    {
        var user1Dto = new CreateUserDto("User1", "u1@mail.com", "hash");
        var user2Dto = new CreateUserDto("User2", "u2@mail.com", "hash");
        var user1 = User.Create(user1Dto).Value;
        var user2 = User.Create(user2Dto).Value;
        _db.Users.AddRange(user1, user2);

        var bookDto = new CreateBookDto("Book", "desc", 0, 0, "ru", "Pub", 2020, 200,
            AgeRestriction.Everyone, null, "ext2", Array.Empty<string>(), Array.Empty<string>());
        var book = Book.Create(bookDto).Value;
        _db.Books.Add(book);

        var reviewDto = new CreateReviewDto("Понравилось!", book.Id);
        var review = Review.Create(reviewDto, user1.Id).Value;
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        var handler = new RemoveReviewHandler(_db);
        var cmd = new RemoveReviewCommand(review.Id, user2.Id);

        var result = await handler.Handle(cmd, CancellationToken.None);

        Assert.That(result.IsSuccess);
        Assert.That(_db.Reviews.Count(), Is.EqualTo(1));
    }
}