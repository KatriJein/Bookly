using Bookly.Application.Handlers.Ratings;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core;
using Core.Dto.Review;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Reviews;

public class UpdateReviewTransactionHandler(IMediator mediator, BooklyDbContext booklyDbContext) : IRequestHandler<UpdateReviewTransactionCommand, Result>
{
    public async Task<Result> Handle(UpdateReviewTransactionCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await booklyDbContext.Database.BeginTransactionAsync(cancellationToken);
        var existingReview = await booklyDbContext.Reviews.FirstOrDefaultAsync(r => r.Id == request.ReviewId, cancellationToken);
        if (existingReview is null) return Result.Failure("Несуществующий отзыв");
        try
        {
            var ratingRes = await mediator.Send(new AddOrUpdateRatingCommand<Book>(db => db.Books, existingReview.BookId,
                request.UserId, request.UpdateReviewWithRatingDto.Rating), cancellationToken);
            if (ratingRes.IsFailure)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Failure(ratingRes.Error);
            }
            var reviewRes = await mediator.Send(new UpdateReviewCommand(new UpdateReviewDto(request.UpdateReviewWithRatingDto.Text),
                request.ReviewId, request.UserId), cancellationToken);
            if (reviewRes.IsFailure)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Failure(reviewRes.Error);
            }
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure("Произошла непредвиденная ошибка при обновлении отзыва");
        }
    }
}

public record UpdateReviewTransactionCommand(UpdateReviewWithRatingDto UpdateReviewWithRatingDto, Guid ReviewId, Guid UserId)
: IRequest<Result>;