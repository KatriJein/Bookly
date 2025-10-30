namespace Core.Dto.Book;

public record GetFullBookDto(Guid Id, string Title, string[] Authors, string[] Genres, string? Description, double Rating, int RatingsCount, string Language,
    string? Publisher, int? PublishmentYear, int PageCount, string AgeRestriction, string? Thumbnail, DateTime CreatedAt);