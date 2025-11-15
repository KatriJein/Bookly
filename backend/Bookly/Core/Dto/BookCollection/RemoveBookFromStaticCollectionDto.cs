namespace Core.Dto.BookCollection;

public record RemoveBookFromStaticCollectionDto(string CollectionName, Guid BookId);