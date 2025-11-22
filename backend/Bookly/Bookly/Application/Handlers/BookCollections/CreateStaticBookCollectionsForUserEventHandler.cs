using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core.Data;
using Core.Dto.BookCollection;
using Core.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace Bookly.Application.Handlers.BookCollections;

public class CreateStaticBookCollectionsForUserEventHandler(BooklyDbContext booklyDbContext, ILogger logger) : INotificationHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        var bookCollections = StaticBookCollectionsData.StaticBookCollectionsNames
            .Select(name => BookCollection.Create(new CreateBookCollectionDto(name, false, notification.UserId), true).Value);
        await booklyDbContext.BookCollections.AddRangeAsync(bookCollections, cancellationToken);
        logger.Information("Статичные коллекции книг для пользователя {userId} успешно добавлены", notification.UserId);
    }
}

public record UserCreatedEvent(Guid UserId) : INotification;