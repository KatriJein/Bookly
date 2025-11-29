using Bookly.Application.Handlers.Preferences;
using Bookly.Infrastructure;
using Core;
using Core.Data;
using Core.Dto.Preferences;
using Core.Payloads;
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
        var result = await mediator.Send(new RemoveBookFromBookCollectionCommand(collection.Id, request.BookId, request.UserId), cancellationToken);
        if (result.IsSuccess && collection.Title == StaticBookCollectionsData.Favorite)
        {
            await mediator.Send(new UpdateUserPreferencesCommand(new PreferencePayloadDto(request.BookId, request.UserId,
                new RemovedFromFavouritesPreferenceActionPayload())), cancellationToken);
            await booklyDbContext.SaveChangesAsync(cancellationToken);
        }
        return Result.Success();
    }
}

public record RemoveBookFromStaticCollectionCommand(string CollectionName, Guid BookId, Guid UserId) : IRequest<Result>;