using Bookly.Domain;
using Bookly.Infrastructure;
using Core;
using Core.Dto.Review;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Reviews;

public class CreateReviewHandler(BooklyDbContext booklyDbContext) : IRequestHandler<CreateReviewCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var book = await booklyDbContext.Books.FirstOrDefaultAsync(b => request.CreateReviewDto.BookId == b.Id, cancellationToken);
        if (book is null) return Result<Guid>.Failure("Несуществующая книга");
        var review = Review.Create(request.CreateReviewDto, request.UserId);
        if (review.IsFailure) return Result<Guid>.Failure(review.Error);
        var entry = await booklyDbContext.Reviews.AddAsync(review.Value, cancellationToken);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entry.Entity.Id);
    }
}

public record CreateReviewCommand(CreateReviewDto CreateReviewDto, Guid UserId) : IRequest<Result<Guid>>;