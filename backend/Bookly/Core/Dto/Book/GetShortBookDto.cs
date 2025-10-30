namespace Core.Dto.Book;

public record GetShortBookDto(Guid Id, string Title, string[] Authors, int? PublishmentYear, double Rating, string[] Genres,
    string? Thumbnail);