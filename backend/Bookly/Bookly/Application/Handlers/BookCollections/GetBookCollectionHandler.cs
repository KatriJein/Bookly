using Bookly.Infrastructure;
using Core.Dto.BookCollection;
using Core.Dto.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.BookCollections;

public class GetBookCollectionHandler(BooklyDbContext booklyDbContext) : IRequestHandler<GetBookCollectionQuery, GetBookCollectionDto?>
{
    public async Task<GetBookCollectionDto?> Handle(GetBookCollectionQuery request, CancellationToken cancellationToken)
    {
        var bookCollection = await booklyDbContext.BookCollections.FirstOrDefaultAsync(bc => request.CollectionId == bc.Id,
                cancellationToken);
        if (bookCollection is null) return null;
        await booklyDbContext.Entry(bookCollection).Reference(bc => bc.User).LoadAsync(cancellationToken);
        await booklyDbContext.Entry(bookCollection).Collection(bc => bc.Books).LoadAsync(cancellationToken);
        var bookCollectionDto = new GetBookCollectionDto(
            bookCollection.Id,
            bookCollection.Title,
            bookCollection.IsStatic,
            bookCollection.IsPublic,
            bookCollection.CoverUrl,
            bookCollection.Rating,
            bookCollection.RatingsCount,
            new GetShortUserDto(bookCollection.UserId, bookCollection.User.Login.Value, bookCollection.User.AvatarKey),
            bookCollection.Books.Count,
            bookCollection.UserId,
            bookCollection.UserRating);
        return bookCollectionDto;
    }
}

public record GetBookCollectionQuery(Guid CollectionId) : IRequest<GetBookCollectionDto?>;