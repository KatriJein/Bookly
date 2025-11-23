using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core;
using Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Ratings;

public class AddOrUpdateRatingHandler<T>(BooklyDbContext booklyDbContext) : IRequestHandler<AddOrUpdateRatingCommand<T>, Result> where T : RateableEntity
{
    public async Task<Result> Handle(AddOrUpdateRatingCommand<T> request, CancellationToken cancellationToken)
    {
        var user = await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null) return Result.Failure("Несуществующий пользователь");
        var entities = request.EntitiesFunc(booklyDbContext);
        var entity = await entities.FirstOrDefaultAsync(e => e.Id == request.EntityId, cancellationToken);
        if (entity is null) return Result.Failure("Несуществующая сущность");
        var rating = await booklyDbContext.Ratings.FirstOrDefaultAsync(r => r.EntityId == request.EntityId && 
                                                                            r.UserId == request.UserId, cancellationToken);
        return rating is null
            ? await CreateNewRatingAsync(entity, request.UserId, request.EntityId, request.Value)
            : await UpdateRatingAsync(entity, rating, request.Value);
    }

    private async Task<Result> CreateNewRatingAsync(T entity, Guid userId, Guid entityId, int value)
    {
        var rating = Rating.Create(userId, entityId, value);
        if (rating.IsFailure) return Result.Failure(rating.Error);
        await booklyDbContext.AddAsync(rating.Value);
        entity.AddNewRating(value);
        try
        {
            await booklyDbContext.SaveChangesAsync();
            return Result.Success();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Failure("Неожиданная ошибка при создании данных об оценке");
        }
    }

    private async Task<Result> UpdateRatingAsync(T entity, Rating rating, int newValue)
    {
        var oldValue = rating.Value;
        var setNewValueRes = rating.UpdateRating(newValue);
        if (setNewValueRes.IsFailure) return Result.Failure(setNewValueRes.Error);
        entity.RefreshRating(oldValue, newValue);
        try
        {
            await booklyDbContext.SaveChangesAsync();
            return Result.Success();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Failure("Неожиданная ошибка при обновлении данных об оценке");
        }
    }
}

public record AddOrUpdateRatingCommand<T>(Func<BooklyDbContext, DbSet<T>> EntitiesFunc, Guid EntityId, Guid UserId, int Value)
: IRequest<Result> where T : RateableEntity;