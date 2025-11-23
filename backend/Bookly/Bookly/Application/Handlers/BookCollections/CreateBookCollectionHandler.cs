using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core;
using Core.Dto.BookCollection;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.BookCollections;

public class CreateBookCollectionHandler(BooklyDbContext booklyDbContext) : IRequestHandler<CreateBookCollectionCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateBookCollectionCommand request, CancellationToken cancellationToken)
    {
        var user = await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.CreateBookCollectionDto.UserId, cancellationToken);
        if (user is null) return Result<Guid>.Failure("Несуществующий пользователь");
        var bookCollection = BookCollection.Create(request.CreateBookCollectionDto, false);
        if (bookCollection.IsFailure) return Result<Guid>.Failure(bookCollection.Error);
        var entry = await booklyDbContext.BookCollections.AddAsync(bookCollection.Value, cancellationToken);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(entry.Entity.Id);
    }
}

public record CreateBookCollectionCommand(CreateBookCollectionDto CreateBookCollectionDto) : IRequest<Result<Guid>>;