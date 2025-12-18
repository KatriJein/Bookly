using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core.Data;
using Core.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Books;

public class ExcludeIrrelevantBooksHandler(BooklyDbContext booklyDbContext) : IRequestHandler<ExcludeIrrelevantBooksCommand, List<Book>>
{
    public async Task<List<Book>> Handle(ExcludeIrrelevantBooksCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId is null || request.UserId == Guid.Empty)
            return request.Books;
        if (!await booklyDbContext.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken))
            return request.Books;
        var entitiesIds = await booklyDbContext.Ratings
            .Where(r => r.UserId == request.UserId)
            .Select(r => r.EntityId)
            .ToHashSetAsync(cancellationToken);
        var readBooksCollectionName = StaticBookCollectionsData.Read;
        var readingBooksCollectionName = StaticBookCollectionsData.Reading;
        var favoriteBooksCollectionName = StaticBookCollectionsData.Favorite;
        var notNeededBookIds = await booklyDbContext.BookCollections
            .Where(bc => bc.IsStatic && bc.UserId == request.UserId && (bc.Title == readBooksCollectionName || bc.Title == readingBooksCollectionName ||
                                                                        bc.Title == favoriteBooksCollectionName))
            .SelectMany(bc => bc.Books.Select(b => b.Id))
            .ToHashSetAsync(cancellationToken);
        var badRecommendationsBookIds = await booklyDbContext.Recommendations
            .Where(r => r.UserId == request.UserId && r.RecommendationStatus == RecommendationStatus.Irrelevant)
            .Select(r => r.BookId)
            .ToHashSetAsync(cancellationToken);
        var userAbsolutelyHatedGenres = await booklyDbContext.UserGenrePreferences
            .Where(g => g.UserId == request.UserId && g.PreferenceType == PreferenceType.Disliked && g.Weight <= -0.995)
            .Select(g => g.GenreId)
            .ToHashSetAsync(cancellationToken);
        entitiesIds.UnionWith(notNeededBookIds);
        entitiesIds.UnionWith(badRecommendationsBookIds);
        return request.Books
            .Where(b =>
                !entitiesIds.Contains(b.Id) &&
                !b.Genres.Any(g => userAbsolutelyHatedGenres.Contains(g.Id)))
            .ToList();
    }
}

public record ExcludeIrrelevantBooksCommand(List<Book> Books, Guid? UserId): IRequest<List<Book>>;