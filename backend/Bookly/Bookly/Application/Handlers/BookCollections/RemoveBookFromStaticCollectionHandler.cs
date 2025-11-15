using Bookly.Infrastructure;
using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.BookCollections;

public class RemoveBookFromStaticCollectionHandler(IMediator mediator, BooklyDbContext booklyDbContext) : IRequestHandler<RemoveBookFromStaticCollectionCommand, Result>
{
    public async Task<Result> Handle(RemoveBookFromStaticCollectionCommand request, CancellationToken cancellationToken)
    {
        var collection = await booklyDbContext.BookCollections.FirstOrDefaultAsync(bc => bc.Title == request.CollectionName
            && bc.UserId == request.UserId && bc.IsStatic, cancellationToken);
        if (collection is null) return Result.Failure("Несуществующая коллекция или не принадлежащая пользователю");
        return await mediator.Send(new RemoveBookFromBookCollectionCommand(collection.Id, request.BookId, request.UserId), cancellationToken);
    }
}

public record RemoveBookFromStaticCollectionCommand(string CollectionName, Guid BookId, Guid UserId) : IRequest<Result>;