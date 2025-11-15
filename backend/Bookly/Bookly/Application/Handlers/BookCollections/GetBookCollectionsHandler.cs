using Bookly.Application.Handlers.Files;
using Bookly.Domain.Models;
using Bookly.Extensions;
using Bookly.Infrastructure;
using Core;
using Core.Dto.BookCollection;
using Core.Dto.File;
using Core.Dto.User;
using Core.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Bookly.Application.Handlers.BookCollections;

public class GetBookCollectionsHandler(IMediator mediator, BooklyDbContext booklyDbContext, IOptionsSnapshot<BooklyOptions> booklyOptions)
    : IRequestHandler<GetBookCollectionsQuery, List<GetBookCollectionDto>>
{
    private const int TrustedRatingsCount = 10;
    
    public async Task<List<GetBookCollectionDto>> Handle(GetBookCollectionsQuery request, CancellationToken cancellationToken)
    {
        List<GetBookCollectionDto> collections = request.UserId is null
            ? await GetCommonBookCollectionsList(request, cancellationToken)
            : await GetSelfBookCollectionsList(request, cancellationToken);

        return collections;
    }

    private async Task<List<GetBookCollectionDto>> GetCommonBookCollectionsList(GetBookCollectionsQuery request, CancellationToken cancellationToken)
    {
        var collectionsRating = await booklyDbContext.BookCollections
            .Where(bc => bc.RatingsCount > 0)
            .Select(bc => bc.Rating)
            .ToListAsync(cancellationToken);
        var averageCollectionsRating = collectionsRating.Count == 0 ? 0 :  collectionsRating.Average();
        var collections = await booklyDbContext.BookCollections
            .Where(bc => bc.IsPublic && !bc.IsStatic)
            .Include(bc => bc.Books)
            .Include(bc => bc.User)
            .OrderByDescending(bc => (bc.Rating * bc.RatingsCount + averageCollectionsRating * TrustedRatingsCount) / (bc.RatingsCount + TrustedRatingsCount))
            .RetrieveNextPage(request.BookCollectionSearchSettingsDto.Page,
                request.BookCollectionSearchSettingsDto.Limit)
            .ToListAsync(cancellationToken);
        
        var tasks = collections.Select(async b =>
        {
            var avatarUrl = await mediator.Send(
                new GetPresignedUrlQuery(
                    new GetObjectPresinedUrlDto(
                        booklyOptions.Value.BooklyFilesStorageBucketName,
                        b.User.AvatarKey)
                ),
                cancellationToken);

            var userInfo = new GetShortUserDto(b.UserId, b.User.Login.Value, avatarUrl);

            return new GetBookCollectionDto(
                b.Id,
                b.Title,
                b.IsStatic,
                b.IsPublic,
                b.CoverUrl,
                b.Rating,
                b.RatingsCount,
                userInfo,
                b.Books.Count,
                b.UserId);
        }).ToArray();

        return (await Task.WhenAll(tasks)).ToList();
    }

    private async Task<List<GetBookCollectionDto>> GetSelfBookCollectionsList(GetBookCollectionsQuery request, CancellationToken cancellationToken)
    {
        var collections = await booklyDbContext.BookCollections
            .Where(bc => bc.UserId == request.UserId!.Value)
            .Include(bc => bc.Books)
            .Include(bc => bc.User)
            .OrderByDescending(bc => bc.IsStatic)
            .ThenByDescending(bc => bc.UpdatedAt)
            .RetrieveNextPage(request.BookCollectionSearchSettingsDto.Page,
                request.BookCollectionSearchSettingsDto.Limit)
            .ToListAsync(cancellationToken);
        var collectionsDto = collections.Select(b =>
            new GetBookCollectionDto(
                b.Id,
                b.Title,
                b.IsStatic,
                b.IsPublic,
                b.CoverUrl,
                b.Rating,
                b.RatingsCount,
                new GetShortUserDto(request.UserId!.Value, b.User.Login.Value, b.User.AvatarKey),
                b.Books.Count,
                b.UserId)
        ).ToList();
        return collectionsDto;
    }
}

public record GetBookCollectionsQuery(BookCollectionSearchSettingsDto BookCollectionSearchSettingsDto, Guid? UserId) : 
    IRequest<List<GetBookCollectionDto>>;