namespace Core.Dto.User;

public record CreateUserDto(string Login, string Email, string PasswordHash);