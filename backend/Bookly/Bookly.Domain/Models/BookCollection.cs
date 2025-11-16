using System.ComponentModel.DataAnnotations.Schema;
using Core;
using Core.Dto.BookCollection;
using Core.Interfaces;

namespace Bookly.Domain.Models;

public class BookCollection : RateableEntity
{
    private readonly List<Book> _books = [];
    public string Title { get; private set; }
    public bool IsStatic { get; private set; }
    public bool IsPublic { get; private set; }
    public string? CoverUrl { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyList<Book> Books => _books;

    public static Result<BookCollection> Create(CreateBookCollectionDto createBookCollectionDto, bool isStatic)
    {
        var collection = new BookCollection
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var setTitleRes = collection.SetTitle(createBookCollectionDto.Title);
        if (setTitleRes.IsFailure) return Result<BookCollection>.Failure(setTitleRes.Error);
        collection.SetIsPublic(createBookCollectionDto.IsPublic);
        collection.SetIsStatic(isStatic);
        collection.SetUserId(createBookCollectionDto.UserId);
        return Result<BookCollection>.Success(collection);
    }

    public Result SetTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length is <= 0 or > 1000)
            return Result.Failure("Пустое или слишком длинное название");
        Title = title;
        return Result.Success();
    }
    
    public void SetIsStatic(bool isStatic) =>  IsStatic = isStatic;
    public void SetIsPublic(bool isPublic) =>  IsPublic = isPublic;
    public void SetUserId(Guid userId) => UserId = userId;
    
    public void Actualize() => UpdatedAt = DateTime.UtcNow;

    public void SetCoverUrl(string? coverUrl)
    {
        if (coverUrl is not { Length: > 0})
        {
            CoverUrl = null;
            return;
        }
        CoverUrl = coverUrl;
    }

    public void AddBookAndUpdateCover(Book book)
    {
        var bookPresented = _books.Any(b => b.Id == book.Id);
        if (bookPresented) return;
        _books.Add(book);
        SetCoverUrl(book.Thumbnail);
    }

    public void RemoveBookAndUpdateCover(Book book)
    {
        _books.Remove(book);
        if (_books.Count == 0)
            CoverUrl = null;
        else
            SetCoverUrl(_books[^1].Thumbnail);
    }
    
    [NotMapped]
    public int? UserRating { get; set; }
}