using Bookly.Infrastructure;
using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.BookCollections;

public class DeleteBookCollectionHandler(BooklyDbContext booklyDbContext) : IRequestHandler<DeleteBookCollectionCommand, Result>
{
    public async Task<Result> Handle(DeleteBookCollectionCommand request, CancellationToken cancellationToken)
    {
        var bookCollection = await booklyDbContext.BookCollections.FirstOrDefaultAsync(
            b => b.Id == request.CollectionId && b.UserId == request.UserId, cancellationToken);
        if (bookCollection is null) return Result.Success();
        if (bookCollection.IsStatic) return Result.Failure("Невозможно удалить статическую коллекцию");
        booklyDbContext.BookCollections.Remove(bookCollection);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteBookCollectionCommand(Guid CollectionId, Guid UserId) : IRequest<Result>;