namespace Core.Dto.BookCollection;

public record CreateBookCollectionDto(string Title, bool IsPublic, Guid UserId);