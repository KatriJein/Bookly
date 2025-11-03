namespace Core.Dto.User;

public record GetFullUserDto(Guid Id, string Login, string Email, string? AvatarUrl, DateTime CreatedAt) : IUserDto;