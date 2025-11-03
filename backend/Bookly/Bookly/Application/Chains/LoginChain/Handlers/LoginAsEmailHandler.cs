using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace Bookly.Application.Chains.LoginChain.Handlers;

public class LoginAsEmailHandler(BooklyDbContext booklyDbContext, ILogger logger) : LoginHandler(null, logger)
{
    protected override async Task<User?> GetUserByLoginAsync(string login, CancellationToken cancellationToken)
    {
        var emailToLower = login.ToLower();
        return await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Email.Value == emailToLower, cancellationToken);
    }
}