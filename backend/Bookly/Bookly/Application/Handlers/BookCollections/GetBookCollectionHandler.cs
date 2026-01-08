using Bookly.Application.Handlers.Books;
using Bookly.Application.Handlers.Files;
using Bookly.Infrastructure;
using Core.Dto.Book;
using Core.Dto.BookCollection;
using Core.Dto.File;
using Core.Dto.User;
using Core.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Bookly.Application.Handlers.BookCollections;

public class GetBookCollectionHandler(IMediator mediator, BooklyDbContext booklyDbContext, IOptionsSnapshot<BooklyOptions> booklyOptions)
    : IRequestHandler<GetBookCollectionQuery, GetFullBookCollectionDto?>
{
    public async Task<GetFullBookCollectionDto?> Handle(GetBookCollectionQuery request, CancellationToken cancellationToken)
    {
        var bookCollection = await booklyDbContext.BookCollections.FirstOrDefaultAsync(bc => request.CollectionId == bc.Id,
                cancellationToken);
        if (bookCollection is null) return null;
        await booklyDbContext.Entry(bookCollection).Reference(bc => bc.User).LoadAsync(cancellationToken);
        var bookSearchOptions = new BookSearchSettingsDto(Limit: int.MaxValue, SearchInBookCollection: request.CollectionId);
        var collectionBooks = await mediator.Send(new GetAllBooksQuery(bookSearchOptions, request.UserId), cancellationToken);
        var getOwnerAvatarUrlDto = new GetObjectPresinedUrlDto(booklyOptions.Value.BooklyFilesStorageBucketName, bookCollection.User.AvatarKey);
        var bookCollectionOwnerAvatar = await mediator.Send(new GetPresignedUrlQuery(getOwnerAvatarUrlDto), cancellationToken);
        var bookCollectionDto = new GetFullBookCollectionDto(
            bookCollection.Id,
            bookCollection.Title,
            bookCollection.IsStatic,
            bookCollection.IsPublic,
            bookCollection.CoverUrl,
            bookCollection.Rating,
            bookCollection.RatingsCount,
            new GetShortUserDto(bookCollection.UserId, bookCollection.User.Login.Value, bookCollectionOwnerAvatar),
            collectionBooks.Count,
            bookCollection.UserId,
            bookCollection.UserRating,
            collectionBooks);
        return bookCollectionDto;
    }
}

public record GetBookCollectionQuery(Guid CollectionId, Guid? UserId) : IRequest<GetFullBookCollectionDto?>;