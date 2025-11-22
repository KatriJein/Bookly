namespace Core.Dto.User;

public record GetShortUserDto(Guid Id, string Login, string? AvatarUrl) : IUserDto;