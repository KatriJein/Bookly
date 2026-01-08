namespace Bookly.Application.Services;

public class SingletonsInitializationService(IServiceScopeFactory serviceScopeFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var initializableSingletons = scope.ServiceProvider.GetServices<IInitializableSingleton>();
        foreach (var initializableSingleton in initializableSingletons)
            await initializableSingleton.InitializeAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}