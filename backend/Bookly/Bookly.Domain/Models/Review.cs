using Bookly.Domain.Models;
using Core;
using Core.Dto.Review;

namespace Bookly.Domain;

public class Review : Entity<Guid>
{
    public string Text { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    public Guid UserId { get; private set; }
    public Guid BookId { get; private set; }
    
    public User User { get; private set; }

    private const int MinLength = 10;
    private const int MaxLength = 1000;

    public static Result<Review> Create(CreateReviewDto createReviewDto, Guid userId)
    {
        var review = new Review();
        var setTextRes = review.SetText(createReviewDto.Text);
        if (setTextRes.IsFailure) return Result<Review>.Failure(setTextRes.Error);
        review.CreatedAt = DateTime.UtcNow;
        review.UpdatedAt = DateTime.UtcNow;
        review.BookId = createReviewDto.BookId;
        review.UserId = userId;
        return Result<Review>.Success(review);
    }

    public Result SetText(string text)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length < MinLength)
            return Result.Failure("Пустой отзыв или слишком маленькая длина отзыва");
        if (text.Length > MaxLength)
            return Result.Failure("Превышена допустимая длина отзыва");
        Text = text;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}