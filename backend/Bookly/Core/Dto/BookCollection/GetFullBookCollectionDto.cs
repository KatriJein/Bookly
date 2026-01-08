using Core.Dto.Book;
using Core.Dto.User;

namespace Core.Dto.BookCollection;

public record GetFullBookCollectionDto(Guid Id, string Title, bool IsStatic, bool IsPublic, string? CoverUrl, double Rating, int RatingsCount,
    GetShortUserDto UserInfo, int BooksCount, Guid UserId, int? UserRating, List<GetShortBookDto> Books);