using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core;
using Core.Dto.BookCollection;
using Core.Dto.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.BookCollections;

public class CreateBookCollectionHandler(BooklyDbContext booklyDbContext) : IRequestHandler<CreateBookCollectionCommand, Result<GetBookCollectionDto>>
{
    public async Task<Result<GetBookCollectionDto>> Handle(CreateBookCollectionCommand request, CancellationToken cancellationToken)
    {
        var user = await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.CreateBookCollectionDto.UserId, cancellationToken);
        if (user is null) return Result<GetBookCollectionDto>.Failure("Несуществующий пользователь");
        var bookCollection = BookCollection.Create(request.CreateBookCollectionDto, false);
        if (bookCollection.IsFailure) return Result<GetBookCollectionDto>.Failure(bookCollection.Error);
        var entry = await booklyDbContext.BookCollections.AddAsync(bookCollection.Value, cancellationToken);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        var bookCollectionDto = new GetBookCollectionDto(
            entry.Entity.Id,
            entry.Entity.Title,
            entry.Entity.IsStatic,
            entry.Entity.IsPublic,
            entry.Entity.CoverUrl,
            entry.Entity.Rating,
            entry.Entity.RatingsCount,
            new GetShortUserDto(user.Id, user.Login.Value, user.AvatarKey),
            0,
            user.Id,
            null);
        return Result<GetBookCollectionDto>.Success(bookCollectionDto);
    }
}

public record CreateBookCollectionCommand(CreateBookCollectionDto CreateBookCollectionDto) : IRequest<Result<GetBookCollectionDto>>;