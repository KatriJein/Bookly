using Bookly.Application.Handlers.Preferences;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core;
using Core.Dto.Preferences;
using Core.Interfaces;
using Core.Payloads;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Ratings;

public class AddOrUpdateRatingHandler<T>(IMediator mediator, BooklyDbContext booklyDbContext) : IRequestHandler<AddOrUpdateRatingCommand<T>, Result> where T : RateableEntity
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
        var oldValue = rating?.Value ?? 0;
        var ratingRes = rating is null
            ? await CreateNewRatingAsync(entity, request.UserId, request.EntityId, request.Value)
            : await UpdateRatingAsync(entity, rating, request.Value);
        if (ratingRes.IsFailure) return Result.Failure(ratingRes.Error);
        if (request.Value != oldValue)
        {
            await mediator.Send(new UpdateUserPreferencesCommand(new PreferencePayloadDto(request.EntityId,
                request.UserId,
                new RatedPreferenceActionPayload(request.Value))), cancellationToken);
        }
        try
        {
            await booklyDbContext.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result.Failure("Неожиданная ошибка при создании данных об оценке");
        }
    }

    private async Task<Result> CreateNewRatingAsync(T entity, Guid userId, Guid entityId, int value)
    {
        var rating = Rating.Create(userId, entityId, value);
        if (rating.IsFailure) return Result.Failure(rating.Error);
        await booklyDbContext.AddAsync(rating.Value);
        entity.AddNewRating(value);
        return Result.Success();
    }

    private async Task<Result> UpdateRatingAsync(T entity, Rating rating, int newValue)
    {
        var oldValue = rating.Value;
        var setNewValueRes = rating.UpdateRating(newValue);
        if (setNewValueRes.IsFailure) return Result.Failure(setNewValueRes.Error);
        entity.RefreshRating(oldValue, newValue);
        return Result.Success();
    }
}

public record AddOrUpdateRatingCommand<T>(Func<BooklyDbContext, DbSet<T>> EntitiesFunc, Guid EntityId, Guid UserId, int Value)
: IRequest<Result> where T : RateableEntity;