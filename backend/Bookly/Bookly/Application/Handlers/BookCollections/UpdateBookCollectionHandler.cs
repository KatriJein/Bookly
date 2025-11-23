using Bookly.Infrastructure;
using Core;
using Core.Dto.BookCollection;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.BookCollections;

public class UpdateBookCollectionHandler(BooklyDbContext booklyDbContext) : IRequestHandler<UpdateBookCollectionCommand, Result>
{
    public async Task<Result> Handle(UpdateBookCollectionCommand request, CancellationToken cancellationToken)
    {
        var bookCollection = await booklyDbContext.BookCollections.FirstOrDefaultAsync(bc 
            => request.CollectionId == bc.Id && request.UserId == bc.UserId, cancellationToken);
        if (bookCollection is null) return Result.Failure("Несуществующая коллекция или не принадлежащая пользователю");
        if (bookCollection.IsStatic) return Result.Failure("Нельзя редактировать статичную коллекцию");
        var setTitleRes = bookCollection.SetTitle(request.UpdateBookCollectionDto.Title ?? bookCollection.Title);
        if (setTitleRes.IsFailure) return Result.Failure(setTitleRes.Error);
        bookCollection.SetIsPublic(request.UpdateBookCollectionDto.IsPublic ?? bookCollection.IsPublic);
        bookCollection.Actualize();
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record UpdateBookCollectionCommand(UpdateBookCollectionDto UpdateBookCollectionDto, Guid CollectionId, Guid UserId) : IRequest<Result>;