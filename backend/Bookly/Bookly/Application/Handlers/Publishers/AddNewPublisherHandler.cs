using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core;
using Core.Dto.Publisher;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace Bookly.Application.Handlers.Publishers;

public class AddNewPublisherHandler(BooklyDbContext booklyDbContext, ILogger logger) : IRequestHandler<AddNewPublisherCommand, Result<Publisher>>
{
    public async Task<Result<Publisher>> Handle(AddNewPublisherCommand request, CancellationToken cancellationToken)
    {
        var existingPublisher = await booklyDbContext.Publishers
            .FirstOrDefaultAsync(p => p.Name.ToLower() == request.CreatePublisherDto.Name.ToLower(), cancellationToken);
        if (existingPublisher != null)
        {
            logger.Information("Издатель {@publisher} уже существует. Пропуск операции создания", request.CreatePublisherDto.Name);
            return Result<Publisher>.Success(existingPublisher);
        }
        var publisher = Publisher.Create(request.CreatePublisherDto);
        if (publisher.IsFailure) return publisher;
        await booklyDbContext.Publishers.AddAsync(publisher.Value, cancellationToken);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        return publisher;
    }
}

public record AddNewPublisherCommand(CreatePublisherDto CreatePublisherDto) : IRequest<Result<Publisher>>;