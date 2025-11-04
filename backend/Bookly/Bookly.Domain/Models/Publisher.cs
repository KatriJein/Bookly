using Core;
using Core.Dto.Publisher;

namespace Bookly.Domain.Models;

public class Publisher : Entity<Guid>
{
    public string Name { get; private set; }

    public static Result<Publisher> Create(CreatePublisherDto createPublisherDto)
    {
        if (string.IsNullOrWhiteSpace(createPublisherDto.Name))
            return Result<Publisher>.Failure("Имя издателя не может быть пустым");
        var publisher = new Publisher()
        {
            Name = createPublisherDto.Name
        };
        return Result<Publisher>.Success(publisher);
    }
}