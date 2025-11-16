using Bookly.Domain.Models;
using Bookly.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace Bookly.Application.Handlers.Books;

public class MarkFavoritesHandler(BooklyDbContext booklyDbContext, ILogger logger) : IRequestHandler<MarkFavoritesCommand>
{
    public async Task<Unit> Handle(MarkFavoritesCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId is null || request.UserId == Guid.Empty) return Unit.Value;
        var userFavorites = await booklyDbContext.BookCollections.FirstOrDefaultAsync(bc => bc.Title == "Избранное"
        && bc.UserId == request.UserId && bc.IsStatic, cancellationToken);
        if (userFavorites is null)
        {
            logger.Warning("У пользователя {userId} не обнаружена коллекция Избранное. Невозможно проставить информацию об избранных книгах",
                request.UserId);
            return Unit.Value;
        }
        await booklyDbContext.Entry(userFavorites).Collection(bc => bc.Books).LoadAsync(cancellationToken);
        var favoritesBooksIds = userFavorites.Books.Select(b => b.Id).ToHashSet();
        foreach (var book in request.Books)
            book.IsFavorite = favoritesBooksIds.Contains(book.Id);
        return Unit.Value;
    }
}

public record MarkFavoritesCommand(List<Book> Books, Guid? UserId = null) : IRequest;