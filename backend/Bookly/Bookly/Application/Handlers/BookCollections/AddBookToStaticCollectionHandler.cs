using Bookly.Application.Handlers.Preferences;
using Bookly.Infrastructure;
using Core;
using Core.Data;
using Core.Dto.Preferences;
using Core.Payloads;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.BookCollections;

public class AddBookToStaticCollectionHandler(IMediator mediator, BooklyDbContext booklyDbContext) : IRequestHandler<AddBookToStaticCollectionCommand, Result>
{
    public async Task<Result> Handle(AddBookToStaticCollectionCommand request, CancellationToken cancellationToken)
    {
        var collection = await booklyDbContext.BookCollections.FirstOrDefaultAsync(bc => bc.Title == request.CollectionName
        && bc.UserId == request.UserId && bc.IsStatic, cancellationToken);
        if (collection is null) return Result.Failure("Несуществующая коллекция или не принадлежащая пользователю");
        var result = await mediator.Send(new AddBookToBookCollectionsCommand([collection.Id], request.BookId, request.UserId), cancellationToken);
        if (result.IsSuccess)
        {
            var payload = DefinePayloadFromCollectionName(collection.Title);
            if (payload is null) return Result.Success();
            await mediator.Send(new UpdateUserPreferencesCommand(new PreferencePayloadDto(request.BookId, request.UserId, payload)), cancellationToken);
            await booklyDbContext.SaveChangesAsync(cancellationToken);
        }
        return Result.Success();
    }

    private IPrerefenceActionPayload? DefinePayloadFromCollectionName(string collectionName)
    {
        return collectionName switch
        {
            StaticBookCollectionsData.Favorite => new AddedToFavouritesPreferenceActionPayload(),
            StaticBookCollectionsData.Reading => new StartedToReadBookPreferenceActionPayload(),
            StaticBookCollectionsData.Read => new ReadBookPreferenceActionPayload(),
            StaticBookCollectionsData.WantToRead => new WantToReadPreferenceActionPayload(),
            _ => null
        };
    }
}

public record AddBookToStaticCollectionCommand(string CollectionName, Guid BookId, Guid UserId) : IRequest<Result>;