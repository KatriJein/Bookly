using Bookly.Application.Mappers;
using Bookly.Infrastructure;
using Core.Dto.Book;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Books;

public class GetBookHandler(BooklyDbContext booklyDbContext) : IRequestHandler<GetBookQuery, GetFullBookDto?>
{
    public async Task<GetFullBookDto?> Handle(GetBookQuery request, CancellationToken cancellationToken)
    {
        var book = await booklyDbContext.Books.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
        if (book == null) return null;
        await booklyDbContext.Entry(book).Collection(b => b.Genres).LoadAsync(cancellationToken);
        await booklyDbContext.Entry(book).Collection(b => b.Authors).LoadAsync(cancellationToken);
        await booklyDbContext.Entry(book).Reference(b => b.Publisher).LoadAsync(cancellationToken);
        return BookMapper.MapBookToFullDto(book);
    }
}

public record GetBookQuery(Guid Id) : IRequest<GetFullBookDto?>;