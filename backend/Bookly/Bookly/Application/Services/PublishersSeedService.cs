using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core.Data;
using Core.Dto.Publisher;
using ILogger = Serilog.ILogger;

namespace Bookly.Application.Services;

public class PublishersSeedService(IServiceScopeFactory serviceScopeFactory, ILogger logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var booklyDbContext = scope.ServiceProvider.GetRequiredService<BooklyDbContext>();
        if (booklyDbContext.Publishers.Any())
        {
            logger.Information("Информация об издателях уже присутствует. Пропуск процедуры наполнения данных");
            return;
        }
        var publishers = PublishersData.BestPublishersList.Select(p => Publisher.Create(new CreatePublisherDto(p)).Value);
        await booklyDbContext.Publishers.AddRangeAsync(publishers, cancellationToken);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        logger.Information("Информация об издателя успешно добавлена в БД");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
    }
}