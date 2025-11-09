using Core.Dto.Author;
using Core.Dto.Genre;

namespace Core.Dto.Book;

public record GetShortBookDto(Guid Id, string Title, GetAuthorDto[] Authors, int? PublishmentYear, double Rating, GetGenreDto[] Genres,
    string? Thumbnail);