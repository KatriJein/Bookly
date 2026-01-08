using Bookly.Application.Mappers;
using Bookly.Application.Services;
using Bookly.Infrastructure;
using Core.Dto.Book;
using Core.Dto.ImpulseSituations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Books;

public class ImpulsePickBookHandler(IMediator mediator, IImpulseSituationsService impulseSituationsService,
    BooklyDbContext booklyDbContext, ILogger<ImpulsePickBookHandler> logger) : IRequestHandler<ImpulsePickBookQuery, GetShortBookDto?>
{
    public async Task<GetShortBookDto?> Handle(ImpulsePickBookQuery request, CancellationToken cancellationToken)
    {
        var situationData = impulseSituationsService.GetImpulseSituation(request.ImpulsePickDto.Situation);
        if (situationData is null)
        {
            logger.LogInformation("Отсутствуют данные по ситуации {situation}",  request.ImpulsePickDto.Situation);
            return null;
        }
        var maxPages = EstimateMaxPages(request.ImpulsePickDto.TimeInMinutes);
        var preferredGenresHashset = situationData.PreferredGenres.ToHashSet();
        var avoidGenresHashset = situationData.AvoidGenres?.ToHashSet() ?? [];
        var suitableBooks = await booklyDbContext.Books.Include(b => b.Genres)
            .Where(b => b.Genres.Any(g => preferredGenresHashset.Contains(g.DisplayName))
                        && b.Genres.All(g => !avoidGenresHashset.Contains(g.DisplayName)))
            .Where(b => b.PageCount <= maxPages)
            .ToListAsync(cancellationToken);
        var relevantBooks = await mediator.Send(new ExcludeIrrelevantBooksCommand(suitableBooks, request.UserId),
            cancellationToken);
        if (relevantBooks.Count == 0)
        {
            logger.LogInformation("К сожалению, отсутствуют книги, которые можно было бы импульсивно предложить");
            return null;
        }
        var impulseBook = relevantBooks.OrderBy(_ => Guid.NewGuid()).First();
        return BookMapper.MapBookToShortBookDto(impulseBook);
    }
    
    private static int EstimateMaxPages(int minutes, double wordsPerMinute = 250, double wordsPerPage = 170)
    {
        return Math.Max(1, (int)Math.Round(minutes * wordsPerMinute / wordsPerPage));
    }
}

public record ImpulsePickBookQuery(ImpulsePickDto ImpulsePickDto, Guid? UserId) : IRequest<GetShortBookDto?>;