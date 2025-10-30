using Core;
using Core.Dto.Book;
using Core.Enums;

namespace Bookly.Domain.Models;

public class Book : Entity<Guid>
{
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public double Rating { get; private set; }
    public int RatingsCount { get; private set; }
    public string Language { get; private set; }
    public string? Publisher { get; private set; }
    public int? PublishmentYear { get; private set; }
    public int PageCount { get; private set; }
    public AgeRestriction AgeRestriction { get; private set; }
    public string? Thumbnail { get; private set; }
    public string ExternalId { get; private set; }
    
    public DateTime CreatedAt { get; private set; }

    private List<Genre> _genres = [];
    private List<Author> _authors = [];
    
    public IReadOnlyCollection<Genre> Genres => _genres;
    public IReadOnlyCollection<Author> Authors => _authors;

    public static Result<Book> Create(CreateBookDto createBookDto)
    {
        var results = new Result[6];
        var book = new Book();
        results[0] = book.SetTitle(createBookDto.Title);
        results[1] = book.SetRating(createBookDto.Rating);
        results[2] = book.SetRatingsCount(createBookDto.RatingsCount);
        results[3] = book.SetLanguage(createBookDto.Language);
        results[4] = book.SetPublishmentYear(createBookDto.PublishmentYear);
        results[5] = book.SetPageCount(createBookDto.PageCount);
        if (results.Any(r => r.IsFailure))
            return Result<Book>.Failure(results.First(r => r.IsFailure).Error);
        book.Description = createBookDto.Description;
        book.Publisher = createBookDto.Publisher;
        book.AgeRestriction = createBookDto.AgeRestriction;
        book.Thumbnail = createBookDto.Thumbnail;
        book.CreatedAt = DateTime.UtcNow;
        book.ExternalId = createBookDto.ExternalId;
        return Result<Book>.Success(book);
    }

    public Result SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure("Название книги не может быть пустым");
        Title = title;
        return Result.Success();
    }
    
    public Result SetRating(double rating)
    {
        if (rating < 0)
            return Result.Failure("Рейтинг книги не может быть отрицательным");
        Rating = rating;
        return Result.Success();
    }
    
    public Result SetRatingsCount(int ratingsCount)
    {
        if (ratingsCount < 0)
            return Result.Failure("Количество оценок книги не может быть отрицательным");
        RatingsCount = ratingsCount;
        return Result.Success();
    }
    
    public Result SetLanguage(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
            return Result.Failure("Язык книги не может быть пустым");
        Language = language;
        return Result.Success();
    }
    
    public Result SetPublishmentYear(int? publishmentYear)
    {
        var currentYear = DateTime.UtcNow.Year;
        if (publishmentYear != null && (publishmentYear < 0 || publishmentYear > currentYear))
            return Result.Failure("Год издания не может быть в будущем или быть отрицательным");
        PublishmentYear = publishmentYear;
        return Result.Success();
    }
    
    public Result SetPageCount(int pageCount)
    {
        if (pageCount < 0)
            return Result.Failure("Количество страниц не может быть отрицательным");
        PageCount = pageCount;
        return Result.Success();
    }
    
    public void AddGenres(IEnumerable<Genre> genres)
    {
        _genres.AddRange(genres);
    }

    public void RemoveGenres(IEnumerable<Genre> genres)
    {
        foreach (var genre in genres)
            _genres.Remove(genre);
    }

    public void AddAuthors(IEnumerable<Author> authors)
    {
        _authors.AddRange(authors);
    }

    public void RemoveAuthors(IEnumerable<Author> authors)
    {
        foreach (var author in authors)
            _authors.Remove(author);
    }
}