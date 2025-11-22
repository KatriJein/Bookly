using Bookly.Application.Mappers;
using Bookly.Infrastructure;
using Core.Dto.User;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookly.Application.Handlers.Users;

public class GetUsersHandler(BooklyDbContext booklyDbContext) : IRequestHandler<GetUsersQuery, List<GetShortUserDto>>
{
    public async Task<List<GetShortUserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await booklyDbContext.Users.ToListAsync(cancellationToken);
        var mappedUsers = users.Select(u => UserMapper.MapUserToShortDto(u, u.AvatarKey ?? "")).ToList();
        return mappedUsers;
    }
}

public record GetUsersQuery() : IRequest<List<GetShortUserDto>>;