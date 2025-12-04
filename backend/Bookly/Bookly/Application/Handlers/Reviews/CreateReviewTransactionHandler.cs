using Bookly.Application.Handlers.Ratings;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core;
using Core.Dto.Review;
using MediatR;

namespace Bookly.Application.Handlers.Reviews;

public class CreateReviewTransactionHandler(IMediator mediator, BooklyDbContext booklyDbContext) : IRequestHandler<CreateReviewTransactionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateReviewTransactionCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await booklyDbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var ratingRes = await mediator.Send(new AddOrUpdateRatingCommand<Book>(db => db.Books, request.CreateReviewWithRatingDto.BookId,
                request.UserId, request.CreateReviewWithRatingDto.Rating), cancellationToken);
            if (ratingRes.IsFailure)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<Guid>.Failure(ratingRes.Error);
            }
            var reviewRes = await mediator.Send(new CreateReviewCommand(new CreateReviewDto(request.CreateReviewWithRatingDto.Text,
                request.CreateReviewWithRatingDto.BookId), request.UserId), cancellationToken);
            if (reviewRes.IsFailure)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<Guid>.Failure(reviewRes.Error);
            }
            await transaction.CommitAsync(cancellationToken);
            return Result<Guid>.Success(reviewRes.Value);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<Guid>.Failure("Произошла непредвиденная ошибка при сохранении отзыва");
        }
    }
}

public record CreateReviewTransactionCommand(CreateReviewWithRatingDto CreateReviewWithRatingDto, Guid UserId) : IRequest<Result<Guid>>;