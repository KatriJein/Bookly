using Bookly.Infrastructure;
using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Reviews;

public class RemoveReviewHandler(BooklyDbContext booklyDbContext) : IRequestHandler<RemoveReviewCommand, Result>
{
    public async Task<Result> Handle(RemoveReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await booklyDbContext.Reviews.FirstOrDefaultAsync(r => r.Id == request.ReviewId && r.UserId == request.UserId,
                cancellationToken);
        if (review is null) return Result.Success();
        booklyDbContext.Reviews.Remove(review);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record RemoveReviewCommand(Guid ReviewId, Guid UserId) : IRequest<Result>;