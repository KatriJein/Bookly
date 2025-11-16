using Core;
using Core.Dto.Rating;

namespace Bookly.Domain.Models;

public class Rating : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public Guid EntityId { get; private set; }
    public int Value { get; private set; }
    
    public User User { get; private set; }

    private const int MinRating = 1;
    private const int MaxRating = 5;

    public static Result<Rating> Create(Guid userId, Guid entityId, int value)
    {
        if (value is < MinRating or > MaxRating)
            return Result<Rating>.Failure("Некорректное значение оценки");
        var rating = new Rating()
        {
            UserId = userId,
            EntityId = entityId,
            Value = value
        };
        return Result<Rating>.Success(rating);
    }

    public Result UpdateRating(int value)
    {
        if (value is < MinRating or > MaxRating)
            return Result.Failure("Некорректное значение оценки");
        Value = value;
        return Result.Success();
    }
}