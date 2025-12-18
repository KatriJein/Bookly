using Bookly.Application.Handlers.Ratings;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core.Dto.Book;
using MediatR;

namespace Bookly.Application.Handlers.Books;

public class ExcludeIrrelevantBooksAndEnrichRelevantWithData(IMediator mediator) 
    : IRequestHandler<ExcludeIrrelevantBooksAndEnrichRelevantWithDataCommand, List<Book>>
{
    public async Task<List<Book>> Handle(ExcludeIrrelevantBooksAndEnrichRelevantWithDataCommand request, CancellationToken cancellationToken)
    {
        var bookSearchSettingsDto = request.BookSimpleSearchSettingsDto;
        var suitableBooks = await mediator.Send(new ExcludeIrrelevantBooksCommand(request.Books, request.UserId), cancellationToken);
        suitableBooks = suitableBooks.DistinctBy(b => b.Id)
            .Skip((bookSearchSettingsDto.Page - 1) * bookSearchSettingsDto.Limit)
            .Take(bookSearchSettingsDto.Limit)
            .ToList();
        await mediator.Send(new MarkFavoritesCommand(suitableBooks, request.UserId), cancellationToken);
        await mediator.Send(new GetRatingQuery<Book>(suitableBooks, request.UserId), cancellationToken);
        return suitableBooks;
    }
}

public record ExcludeIrrelevantBooksAndEnrichRelevantWithDataCommand(List<Book> Books, Guid? UserId, BookSimpleSearchSettingsDto BookSimpleSearchSettingsDto)
    : IRequest<List<Book>>;