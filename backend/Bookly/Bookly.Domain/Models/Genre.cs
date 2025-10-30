using Core;
using Core.Dto.Genre;

namespace Bookly.Domain.Models;

public class Genre : Entity<Guid>
{
    public string Name { get; private set; }
    public string DisplayName { get; private set; }

    private readonly List<Book> _books = [];

    public IReadOnlyCollection<Book> Books => _books;

    public static Result<Genre> Create(CreateGenreDto createGenreDto)
    {
        if (string.IsNullOrWhiteSpace(createGenreDto.Name) || string.IsNullOrWhiteSpace(createGenreDto.DisplayName))
            return Result<Genre>.Failure("Название жанра или его отображение не может быть пустым");
        
        var genre = new Genre()
        {
            Name = createGenreDto.Name,
            DisplayName = createGenreDto.DisplayName
        };
        return Result<Genre>.Success(genre);
    }
}