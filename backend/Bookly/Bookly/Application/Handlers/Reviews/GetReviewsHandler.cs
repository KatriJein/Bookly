using System.Collections.Concurrent;
using Bookly.Application.Handlers.Files;
using Bookly.Application.Handlers.Ratings;
using Bookly.Domain;
using Bookly.Domain.Models;
using Bookly.Extensions;
using Bookly.Infrastructure;
using Core.Dto.File;
using Core.Dto.Review;
using Core.Dto.User;
using Core.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Bookly.Application.Handlers.Reviews;

public class GetReviewsHandler(IMediator mediator, BooklyDbContext booklyDbContext, IOptionsSnapshot<BooklyOptions> booklyOptions) 
    : IRequestHandler<GetReviewsQuery, List<GetReviewDto>>
{
    public async Task<List<GetReviewDto>> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
    {
        var shouldTakeOnlyUserReviews = request.UserId != Guid.Empty && request.BookId == null;
        var reviews = shouldTakeOnlyUserReviews
            ? booklyDbContext.Reviews.Where(r => r.UserId == request.UserId)
            : booklyDbContext.Reviews;
        var reviewsPage = await reviews
            .Where(r => request.BookId == null || r.BookId ==  request.BookId)
            .Include(r => r.User)
            .OrderByDescending(r => r.UpdatedAt)
            .RetrieveNextPage(request.SearchSettingsDto.Page, request.SearchSettingsDto.Limit)
            .ToListAsync(cancellationToken);
        var reviewsInfo = new Dictionary<Guid, ReviewAdditionalInfo>();
        await FillInReviewsAdditionalInfo(reviewsPage, reviewsInfo, cancellationToken);
        var reviewsDto = reviewsPage.Select(r =>
        {
            var additionalInfo = reviewsInfo[r.Id];
            return new GetReviewDto(r.Text, DateOnly.FromDateTime(r.CreatedAt), DateOnly.FromDateTime(r.UpdatedAt), additionalInfo.Rating, additionalInfo.UserInfo);
        }).ToList();
        return reviewsDto;
    }

    private async Task FillInReviewsAdditionalInfo(List<Review> reviews, Dictionary<Guid, ReviewAdditionalInfo> reviewsInfo,
        CancellationToken cancellationToken)
    {
        var userInfos = new ConcurrentDictionary<Guid, GetShortUserDto>();
        var users = reviews.Select(r => r.User)
            .DistinctBy(u => u.Id);
        var fetchUserInfosTasks = users.Select(async user =>
        {
            if (userInfos.ContainsKey(user.Id)) return;
            var getPresignedUrlDto = new GetObjectPresinedUrlDto(booklyOptions.Value.BooklyFilesStorageBucketName, user.AvatarKey);
            var userAvatarUrl = await mediator.Send(new GetPresignedUrlQuery(getPresignedUrlDto), cancellationToken);
            var userDto = new GetShortUserDto(user.Id, user.Login.Value, userAvatarUrl);
            userInfos[userDto.Id] = userDto;
        });
        await Task.WhenAll(fetchUserInfosTasks);
        var requiredRatingPairs = reviews
            .Select(r => (r.UserId, r.BookId))
            .ToHashSet();
        var ratings = (await booklyDbContext.Ratings
            .ToListAsync(cancellationToken))
            .Where(r => requiredRatingPairs.Contains((r.UserId, r.EntityId)))
            .ToDictionary(r => (r.UserId, r.EntityId), r => r.Value);
        foreach (var review in reviews)
        {
            var userDto = userInfos[review.UserId];
            var hasRating = ratings.TryGetValue((review.UserId, review.BookId), out var rating);
            reviewsInfo[review.Id] = new ReviewAdditionalInfo(userDto, hasRating ? rating : null);
        }
    }
}

public record GetReviewsQuery(ReviewSearchSettingsDto SearchSettingsDto, Guid? BookId, Guid UserId) : IRequest<List<GetReviewDto>>;

public record ReviewAdditionalInfo(GetShortUserDto UserInfo, int? Rating);