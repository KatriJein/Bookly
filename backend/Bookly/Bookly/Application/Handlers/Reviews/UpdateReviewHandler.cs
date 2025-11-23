using Bookly.Infrastructure;
using Core;
using Core.Dto.Review;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Reviews;

public class UpdateReviewHandler(BooklyDbContext booklyDbContext) : IRequestHandler<UpdateReviewCommand, Result>
{
    public async Task<Result> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await booklyDbContext.Reviews.FirstOrDefaultAsync(r => r.Id == request.ReviewId && r.UserId == request.UserId,
            cancellationToken);
        if (review is null) return Result.Failure("Несуществующий или не принадлежащий пользователю отзыв");
        var setTextRes = review.SetText(request.UpdateReviewDto.Text);
        if (setTextRes.IsFailure) return Result.Failure(setTextRes.Error);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record UpdateReviewCommand(UpdateReviewDto UpdateReviewDto, Guid ReviewId, Guid UserId) : IRequest<Result>;