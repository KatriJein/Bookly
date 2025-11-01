using Bookly.Domain.Models;
using Core.Dto.Book;
using Core.Mappers;

namespace Bookly.Application.Mappers;

public static class BookMapper
{
    public static GetFullBookDto MapBookToFullDto(Book book)
    {
        var authors = book.Authors.Select(a => a.DisplayName).ToArray();
        var genres = book.Genres.Select(g => g.DisplayName).ToArray();
        var ageRestriction = EnumMapper.MapAgeRestrictionEnumToString(book.AgeRestriction);
        return new GetFullBookDto(
            book.Id,
            book.Title,
            authors,
            genres,
            book.Description,
            book.Rating,
            book.RatingsCount,
            book.Language,
            book.Publisher,
            book.PublishmentYear,
            book.PageCount,
            ageRestriction,
            book.Thumbnail,
            book.CreatedAt
        );
    }

    public static GetShortBookDto MapBookToShortBookDto(Book book)
    {
        var authors = book.Authors.Select(a => a.DisplayName).ToArray();
        var genres = book.Genres.Select(g => g.DisplayName).ToArray();
        return new GetShortBookDto
        (
            book.Id,
            book.Title,
            authors,
            book.PublishmentYear,
            book.Rating,
            genres,
            book.Thumbnail
        );
    }
}