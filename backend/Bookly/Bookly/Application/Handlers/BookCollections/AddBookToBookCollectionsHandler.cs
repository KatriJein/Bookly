using Bookly.Infrastructure;
using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.BookCollections;

public class AddBookToBookCollectionsHandler(BooklyDbContext booklyDbContext) : IRequestHandler<AddBookToBookCollectionsCommand, Result>
{
    public async Task<Result> Handle(AddBookToBookCollectionsCommand request, CancellationToken cancellationToken)
    {
        var book = await booklyDbContext.Books.FirstOrDefaultAsync(b => b.Id == request.BookId, cancellationToken);
        if (book is null) return Result.Failure("Несуществующая книга");
        var user = await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null) return Result.Failure("Несуществующий пользователь");
        var collectionsIdsHashset = request.CollectionIds.ToHashSet();
        var collections = await booklyDbContext.BookCollections
            .Where(bc => collectionsIdsHashset.Contains(bc.Id) && bc.UserId == request.UserId)
            .Include(bc => bc.Books)
            .ToListAsync(cancellationToken);
        foreach (var collection in collections)
            collection.AddBookAndUpdateCover(book);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

public record AddBookToBookCollectionsCommand(List<Guid> CollectionIds, Guid BookId, Guid UserId) : IRequest<Result>;