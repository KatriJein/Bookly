using Bookly.Infrastructure;
using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Books;

public class DeleteBookHandler(BooklyDbContext booklyDbContext) : IRequestHandler<DeleteBookCommand, Result>
{
    public async Task<Result> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        await booklyDbContext.Books.Where(b => b.Id == request.Id).ExecuteDeleteAsync(cancellationToken);
        return Result.Success();
    }
}

public record DeleteBookCommand(Guid Id) : IRequest<Result>;