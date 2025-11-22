using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace Bookly.Application.Chains.LoginChain.Handlers;

public class LoginAsUsernameHandler(BooklyDbContext booklyDbContext, LoginAsEmailHandler? next, ILogger logger) : LoginHandler(next, logger)
{
    protected override async Task<User?> GetUserByLoginAsync(string login, CancellationToken cancellationToken)
    {
        return await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Login.Value == login, cancellationToken);
    }
}