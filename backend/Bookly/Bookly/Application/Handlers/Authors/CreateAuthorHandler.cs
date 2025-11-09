using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Core;
using Core.Dto.Author;
using MediatR;
using ILogger = Serilog.ILogger;

namespace Bookly.Application.Handlers.Authors;

public class CreateAuthorHandler(IMediator mediator, BooklyDbContext booklyDbContext, ILogger logger) : IRequestHandler<CreateAuthorCommand, Result<Author>>
{
    public async Task<Result<Author>> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var authorFormWithStandardName = await mediator.Send(new GetAuthorQuery(request.CreateAuthorDto.Name), cancellationToken);
        var authorNameToBestFormat = AuthorUtils.AuthorNameToBestFormat(request.CreateAuthorDto.Name);
        var authorFormWithBestName = await mediator.Send(new GetAuthorQuery(authorNameToBestFormat), cancellationToken);
        if (authorFormWithBestName != null || authorFormWithStandardName != null)
        {
            logger.Information("Автор {@author} найден. Пропуск операции создания", request.CreateAuthorDto.Name);
            return Result<Author>.Success((authorFormWithStandardName ?? authorFormWithBestName)!);
        }
        var createAuthorResult = Author.Create(new CreateAuthorDto(authorNameToBestFormat,
            authorNameToBestFormat == request.CreateAuthorDto.Name ? authorNameToBestFormat : request.CreateAuthorDto.Name));
        if (createAuthorResult.IsFailure)
        {
            logger.Error("Не удалось создать модель автора: {@error}", createAuthorResult.Error);
            return Result<Author>.Failure(createAuthorResult.Error);
        }

        var entity = await booklyDbContext.Authors.AddAsync(createAuthorResult.Value, cancellationToken);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        logger.Information("Автор {@author} успешно создан", request.CreateAuthorDto.Name);
        return Result<Author>.Success(entity.Entity);
    }
}

public record CreateAuthorCommand(CreateAuthorDto CreateAuthorDto) : IRequest<Result<Author>>;