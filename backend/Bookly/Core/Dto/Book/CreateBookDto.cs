using Core.Enums;

namespace Core.Dto.Book;

public record CreateBookDto(string Title, string? Description, double Rating, int RatingsCount, string Language,
    string? Publisher, int? PublishmentYear, int PageCount, AgeRestriction AgeRestriction, string? Thumbnail, string ExternalId);