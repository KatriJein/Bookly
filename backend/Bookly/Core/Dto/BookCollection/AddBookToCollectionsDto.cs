namespace Core.Dto.BookCollection;

public record AddBookToCollectionsDto(List<Guid> CollectionIds, Guid BookId);