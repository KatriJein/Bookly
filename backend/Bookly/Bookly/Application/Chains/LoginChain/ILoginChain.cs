using Bookly.Domain.Models;

namespace Bookly.Application.Chains.LoginChain;

public interface ILoginChain
{
    Task<User?> FindUserByLoginAsync(string login, CancellationToken cancellationToken);
}