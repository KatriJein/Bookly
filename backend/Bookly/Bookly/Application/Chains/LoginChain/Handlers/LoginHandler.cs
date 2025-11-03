using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace Bookly.Application.Chains.LoginChain.Handlers;

public abstract class LoginHandler(LoginHandler? next, ILogger logger) : ILoginHandler
{
    public async Task<User?> Handle(string login, CancellationToken cancellationToken)
    {
        var user = await GetUserByLoginAsync(login, cancellationToken);
        if (user != null)
        {
            logger.Information("Обработчик логина {@handlerName} выполнился успешно.", nameof(GetType));
            return user;
        }
        logger.Information("Обработчик логина {@handlerName} не обнаружил пользователя", nameof(GetType));
        return await (next?.GetUserByLoginAsync(login, cancellationToken) ?? Task.FromResult<User?>(null));
    }

    protected abstract Task<User?> GetUserByLoginAsync(string login, CancellationToken cancellationToken);
}

public interface ILoginHandler
{
    Task<User?> Handle(string login, CancellationToken cancellationToken = default);
}