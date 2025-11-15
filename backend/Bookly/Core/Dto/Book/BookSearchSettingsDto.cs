using Core.Enums;

namespace Core.Dto.Book;

public record BookSearchSettingsDto(int Page = 1, int Limit = 30, Guid? SearchInBookCollection = null, string? SearchByTitle = null, string[]? SearchByAuthors = null,
    string[]? SearchByGenres = null, string? SearchByPublisher = null, double? SearchByRating = null, VolumeSizePreference? SearchByVolumeSizePreference = null,
    BooksOrderOption? BooksOrderOption = BooksOrderOption.ByRatingDescending);