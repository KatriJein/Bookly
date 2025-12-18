namespace Core.Dto.Review;

public record CreateReviewWithRatingDto(string Text, int Rating, Guid BookId);