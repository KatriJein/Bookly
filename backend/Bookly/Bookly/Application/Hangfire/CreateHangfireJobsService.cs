using Bookly.Application.Services.ApiScrapers;
using Core;
using Hangfire;

namespace Bookly.Application.Hangfire;

public class CreateHangfireJobsService(IRecurringJobManager recurringJobManager, IServiceScopeFactory serviceScopeFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var booksApiScraperService = scope.ServiceProvider.GetRequiredService<IBooksApiScraperService>();
        recurringJobManager.AddOrUpdate(Const.BooksApiScrapingJob,
            () => booksApiScraperService.ScrapeNextAsync(), "*/30 * * * * *");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
    }
}

