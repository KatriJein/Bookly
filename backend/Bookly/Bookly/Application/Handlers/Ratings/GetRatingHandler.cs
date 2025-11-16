using Bookly.Infrastructure;
using Core;
using Core.Dto.Rating;
using Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Ratings;

public class GetRatingHandler<T>(BooklyDbContext booklyDbContext) : IRequestHandler<GetRatingQuery<T>, Result<RatingDto?>> where T : RateableEntity
{
    public async Task<Result<RatingDto?>> Handle(GetRatingQuery<T> request, CancellationToken cancellationToken)
    {
        if (request.UserId is null) return Result<RatingDto?>.Failure("Не указан UserId для получения оценки пользователя");
        var user = await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null) return Result<RatingDto?>.Failure("Несуществующий пользователь");
        var entities = request.EntitiesFunc(booklyDbContext);
        var entity = await entities.FirstOrDefaultAsync(e => e.Id == request.EntityId, cancellationToken);
        if (entity is null) return Result<RatingDto?>.Failure("Несуществующая сущность");
        var rating = await booklyDbContext.Ratings.FirstOrDefaultAsync(r => r.EntityId == request.EntityId &&
                                                                            r.UserId == request.UserId, cancellationToken);
        return rating is null ? Result<RatingDto?>.Success(null) : Result<RatingDto?>.Success(new RatingDto(rating.Value));
    }
}

public record GetRatingQuery<T>(Func<BooklyDbContext, DbSet<T>> EntitiesFunc, Guid? UserId, Guid EntityId) : IRequest<Result<RatingDto?>>
where T : RateableEntity;