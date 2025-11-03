namespace Core.Dto.User;

public record UpdatePasswordDto(string OldPassword, string NewPassword);