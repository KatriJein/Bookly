using Bookly.Infrastructure;
using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.BookCollections;

public class RemoveBookFromBookCollectionHandler(BooklyDbContext booklyDbContext) : IRequestHandler<RemoveBookFromBookCollectionCommand, Result>
{
    public async Task<Result> Handle(RemoveBookFromBookCollectionCommand request, CancellationToken cancellationToken)
    {
        var book = await booklyDbContext.Books.FirstOrDefaultAsync(b => b.Id == request.BookId, cancellationToken);
        if (book is null) return Result.Failure("Несуществующая книга");
        var user = await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null) return Result.Failure("Несуществующий пользователь");
        var collection = await booklyDbContext.BookCollections.FirstOrDefaultAsync(bc =>
            bc.UserId == request.UserId && bc.Id == request.CollectionId, cancellationToken);
        if (collection is null) return Result.Failure("Несуществующая или не принадлежащая пользователю коллекция");
        await booklyDbContext.Entry(collection).Collection(c => c.Books).LoadAsync(cancellationToken);
        collection.RemoveBookAndUpdateCover(book);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record RemoveBookFromBookCollectionCommand(Guid CollectionId, Guid BookId, Guid UserId) : IRequest<Result>;