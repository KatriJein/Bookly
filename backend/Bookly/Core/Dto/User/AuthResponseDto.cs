namespace Core.Dto.User;

public record AuthResponseDto(Guid Id, string Login, string Email, string AvatarUrl, string AccessToken, bool TookEntrySurvey);