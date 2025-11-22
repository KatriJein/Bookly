using Core.Dto.Author;
using Core.Dto.Genre;
using Core.Enums;

namespace Core.Dto.Book;

public record GetShortBookDto(Guid Id, string Title, GetAuthorDto[] Authors, int? PublishmentYear, double Rating, GetGenreDto[] Genres, 
    string? Thumbnail, string Language, string AgeRestriction, bool IsFavorite, int? UserRating);