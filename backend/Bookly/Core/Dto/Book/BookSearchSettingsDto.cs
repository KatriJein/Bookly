using Core.Enums;

namespace Core.Dto.Book;

public record BookSearchSettingsDto(int Page = 1, int Limit = 30, string? SearchByTitle = null, string[]? SearchByAuthors = null,
    string[]? SearchByGenres = null, double? SearchByRating = null, BooksOrderOption? BooksOrderOption = BooksOrderOption.ByRatingDescending);