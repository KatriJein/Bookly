using Bookly.Application.Chains.LoginChain.Handlers;
using Bookly.Domain.Models;
using ILogger = Serilog.ILogger;

namespace Bookly.Application.Chains.LoginChain;

public class LoginChain(LoginAsUsernameHandler startHandler, ILogger logger) : ILoginChain
{
    public async Task<User?> FindUserByLoginAsync(string login, CancellationToken cancellationToken)
    {
        var user = await startHandler.Handle(login, cancellationToken);
        if (user != null) return user;
        logger.Information("Все обработчики логина не обнаружили пользователя");
        return null;
    }
}