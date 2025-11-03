using Bookly.Domain.Models;
using Core.Dto.User;

namespace Bookly.Application.Mappers;

public static class UserMapper
{
    public static GetFullUserDto MapUserToFullDto(User user, string presignedUrl)
    {
        return new GetFullUserDto(
            user.Id,
            user.Login.Value,
            user.Email.Value,
            presignedUrl,
            user.CreatedAt);
    }

    public static GetShortUserDto MapUserToShortDto(User user, string presignedUrl)
    {
        return new GetShortUserDto(user.Id, user.Login.Value, presignedUrl);
    }

    public static AuthResponseDto MapUserToAuthResponseDto(User user, string presignedUrl)
    {
        return new AuthResponseDto(user.Id, user.Login.Value, user.Email.Value, presignedUrl);
    }
}