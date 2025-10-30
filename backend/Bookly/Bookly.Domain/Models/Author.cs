using Core;
using Core.Dto.Author;

namespace Bookly.Domain.Models;

public class Author : Entity<Guid>
{
    public string FullName { get; private set; }
    public string DisplayName { get; private set; }

    private readonly List<Book> _books = [];
    
    public IReadOnlyList<Book> Books => _books;

    public static Result<Author> Create(CreateAuthorDto createAuthorDto)
    {
        if (string.IsNullOrWhiteSpace(createAuthorDto.FullName))
            return Result<Author>.Failure("Имя автора не может быть пустым");
        if (string.IsNullOrWhiteSpace(createAuthorDto.DisplayName))
            return Result<Author>.Failure("Отображаемое имя не может быть пустым");
        var author = new Author()
        {
            FullName = createAuthorDto.FullName,
            DisplayName = createAuthorDto.DisplayName
        };
        return Result<Author>.Success(author);
    }
}