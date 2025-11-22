using Core.Dto.User;

namespace Core.Dto.Review;

public record GetReviewDto(string Text, DateOnly CreatedAt, DateOnly UpdatedAt, int? Rating, GetShortUserDto UserInfo);