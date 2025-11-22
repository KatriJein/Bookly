namespace Core.Dto.BookCollection;

public record RemoveBookFromCollectionDto(Guid CollectionId, Guid BookId);