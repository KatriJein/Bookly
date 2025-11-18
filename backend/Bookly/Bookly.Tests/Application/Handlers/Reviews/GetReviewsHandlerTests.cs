using Bookly.Application.Handlers.Files;
using Bookly.Application.Handlers.Reviews;
using Bookly.Domain;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core.Dto.Book;
using Core.Dto.Review;
using Core.Dto.User;
using Core.Enums;
using Core.Options;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;


namespace Bookly.Tests.Application.Handlers.Reviews
{
    [TestFixture]
    public class GetReviewsHandlerTests
    {
        private BooklyDbContext _db = null!;
        private Mock<IMediator> _mediatorMock = null!;
        private Mock<IOptionsSnapshot<BooklyOptions>> _optionsMock = null!;

        [SetUp]
        public void Setup()
        {
            _db = DatabaseUtils.CreateDbContext();
            _mediatorMock = new Mock<IMediator>();
            _optionsMock = new Mock<IOptionsSnapshot<BooklyOptions>>();
            _optionsMock.Setup(o => o.Value)
                .Returns(new BooklyOptions { BooklyFilesStorageBucketName = "bucket" });
        }

        [TearDown]
        public void Teardown()
        {
            _db.Dispose();
        }

        [Test]
        public async Task Handle_ReturnsAllReviews_WhenUserNotAuthorized()
        {
            var users = Enumerable.Range(1, 3)
                .Select(i => User.Create(new CreateUserDto($"User{i}", $"u{i}@mail.com", "pwd")).Value)
                .ToList();
            _db.Users.AddRange(users);

            var book = Book.Create(new CreateBookDto("Book", "desc", 0, 0, "ru", "Pub", 2022, 200,
                AgeRestriction.Everyone, null, "ext", Array.Empty<string>(), Array.Empty<string>())).Value;
            _db.Books.Add(book);

            var reviews = users.Select(u => Review.Create(
                new CreateReviewDto($"Текст отзыва пользователя {u.Login.Value}", book.Id), u.Id).Value);
            _db.Reviews.AddRange(reviews);
            await _db.SaveChangesAsync();

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPresignedUrlQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("https://avatar.example.com");

            var handler = new GetReviewsHandler(_mediatorMock.Object, _db, _optionsMock.Object);
            var query = new GetReviewsQuery(new ReviewSearchSettingsDto(Page: 1, Limit: 10), book.Id, Guid.Empty);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result.All(r => r.UserInfo.AvatarUrl == "https://avatar.example.com"));
            Assert.That(result.All(r => r.Rating == null));
        }

        [Test]
        public async Task Handle_ReturnsOnlyUserReviews_WhenOnlyMineTrue()
        {
            var user1 = User.Create(new CreateUserDto("User1", "u1@mail.com", "pwd")).Value;
            var user2 = User.Create(new CreateUserDto("User2", "u2@mail.com", "pwd")).Value;
            _db.Users.AddRange(user1, user2);

            var book = Book.Create(new CreateBookDto("Book", "desc", 0, 0, "ru", "Pub", 2020, 300,
                AgeRestriction.Everyone, null, "ext", Array.Empty<string>(), Array.Empty<string>())).Value;
            _db.Books.Add(book);

            var ownReview = Review.Create(new CreateReviewDto("Мой отзыв на книгу", book.Id), user1.Id).Value;
            var foreignReview = Review.Create(new CreateReviewDto("Чужой отзыв", book.Id), user2.Id).Value;
            _db.Reviews.AddRange(ownReview, foreignReview);

            _db.Ratings.Add(Rating.Create(user1.Id, book.Id, 5).Value);
            _db.Ratings.Add(Rating.Create(user2.Id, book.Id, 3).Value);
            await _db.SaveChangesAsync();

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPresignedUrlQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("https://avatar.example.com");

            var handler = new GetReviewsHandler(_mediatorMock.Object, _db, _optionsMock.Object);
            var query = new GetReviewsQuery(
                new ReviewSearchSettingsDto(Page: 1, Limit: 10),
                null, user1.Id);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result.Count, Is.EqualTo(1));
            var review = result.First();
            Assert.That(review.UserInfo.Login, Is.EqualTo(user1.Login.Value));
            Assert.That(review.Rating, Is.EqualTo(5));
        }

        [Test]
        public async Task Handle_PaginatesLargeAmountOfReviews()
        {
            var user = User.Create(new CreateUserDto("User", "u@mail.com", "pwd")).Value;
            _db.Users.Add(user);

            var book = Book.Create(new CreateBookDto("Book", "desc", 0, 0, "ru", "Pub", 2021, 250,
                AgeRestriction.Everyone, null, "ext3", Array.Empty<string>(), Array.Empty<string>())).Value;
            _db.Books.Add(book);

            var reviews = Enumerable.Range(1, 50)
                .Select(i => Review.Create(new CreateReviewDto($"Отзыв {i} на книгу", book.Id), user.Id).Value)
                .ToList();
            _db.Reviews.AddRange(reviews);
            await _db.SaveChangesAsync();

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPresignedUrlQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("url");

            var handler = new GetReviewsHandler(_mediatorMock.Object, _db, _optionsMock.Object);
            var query = new GetReviewsQuery(new ReviewSearchSettingsDto(Page: 2, Limit: 10), book.Id, user.Id);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result.Count, Is.EqualTo(10));
            var ordered = result.OrderByDescending(r => r.UpdatedAt).ToList();
            Assert.That(result.Select(r => r.Text), Is.EqualTo(ordered.Select(r => r.Text)));
        }

        [Test]
        public async Task Handle_WorksCorrectly_WhenRatingsAbsent()
        {
            var user = User.Create(new CreateUserDto("User", "mail@mail.com", "pwd")).Value;
            _db.Users.Add(user);

            var book = Book.Create(new CreateBookDto("Book", "d", 0, 0, "ru", "Pub", 2019, 300,
                AgeRestriction.Everyone, null, "ext4", Array.Empty<string>(), Array.Empty<string>())).Value;
            _db.Books.Add(book);

            var review = Review.Create(new CreateReviewDto("Отзыв без оценки", book.Id), user.Id).Value;
            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPresignedUrlQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("avatar");

            var handler = new GetReviewsHandler(_mediatorMock.Object, _db, _optionsMock.Object);
            var query = new GetReviewsQuery(new ReviewSearchSettingsDto(), null, user.Id);

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.That(result.Single().Rating, Is.Null);
        }
    }
}