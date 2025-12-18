using Bookly.Application.Handlers.Preferences;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core;
using Core.Dto.Preferences;
using Core.Dto.Recommendation;
using Core.Payloads;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Recommendations;

public class AddRecommendationStatusHandler(IMediator mediator, BooklyDbContext booklyDbContext) : IRequestHandler<AddRecommendationStatusCommand, Result>
{
    public async Task<Result> Handle(AddRecommendationStatusCommand request, CancellationToken cancellationToken)
    {
        var userExists = await booklyDbContext.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists) return Result.Failure("Несуществующий пользователь");
        var statusExists = await booklyDbContext.Recommendations.AnyAsync(r => r.UserId == request.UserId &&
                                                                               r.BookId == request.RecommendationDto.BookId, cancellationToken);
        if (statusExists) return Result.Failure("Мнение о данной рекомендации уже проставлено");
        var recommendation = Recommendation.Create(request.RecommendationDto, request.UserId);
        await booklyDbContext.AddAsync(recommendation, cancellationToken);
        await mediator.Send(new UpdateUserPreferencesCommand(new PreferencePayloadDto(request.RecommendationDto.BookId,
                request.UserId,
                new RespondedToRecommendationPreferenceActionPayload(request.RecommendationDto.RecommendationStatus))),
            cancellationToken);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record AddRecommendationStatusCommand(RecommendationDto RecommendationDto, Guid UserId) : IRequest<Result>;