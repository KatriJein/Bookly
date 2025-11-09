using Core;
using Core.Dto.Author;

namespace Bookly.Domain.Models;

public class Author : Entity<Guid>
{
    public string Name { get; private set; }
    public string DisplayName { get; private set; }

    private readonly List<Book> _books = [];
    private readonly List<UserAuthorPreference> _userAuthorPreferences = [];
    
    public IReadOnlyList<Book> Books => _books;
    public IReadOnlyCollection<UserAuthorPreference> UserAuthorPreferences => _userAuthorPreferences;

    public static Result<Author> Create(CreateAuthorDto createAuthorDto)
    {
        if (string.IsNullOrWhiteSpace(createAuthorDto.Name))
            return Result<Author>.Failure("Имя автора не может быть пустым");
        if (string.IsNullOrWhiteSpace(createAuthorDto.DisplayName))
            return Result<Author>.Failure("Отображаемое имя не может быть пустым");
        var author = new Author()
        {
            Id = Guid.NewGuid(),
            Name = createAuthorDto.Name,
            DisplayName = createAuthorDto.DisplayName
        };
        return Result<Author>.Success(author);
    }
}