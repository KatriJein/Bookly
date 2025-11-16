using Core.Dto.Author;
using Core.Dto.Genre;

namespace Core.Dto.Book;

public record GetFullBookDto(Guid Id, string Title, GetAuthorDto[] Authors, GetGenreDto[] Genres, string? Description, double Rating, int RatingsCount, string Language,
    string Publisher, int? PublishmentYear, int PageCount, string AgeRestriction, string? Thumbnail, DateTime CreatedAt, bool IsFavorite);