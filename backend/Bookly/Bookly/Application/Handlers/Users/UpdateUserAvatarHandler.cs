using Bookly.Application.Services.Files;
using Bookly.Infrastructure;
using Core;
using Core.Dto.File;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace Bookly.Application.Handlers.Users;

public class UpdateUserAvatarHandler(BooklyDbContext booklyDbContext, IFilesService filesService, ILogger logger) : IRequestHandler<UpdateUserAvatarCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateUserAvatarCommand request, CancellationToken cancellationToken)
    {
        var fileExtension = Path.GetExtension(request.File.File.FileName);
        if (!Const.SupportedAvatarFileExtensions.Contains(fileExtension))
            return Result<string>.Failure($"Формат файла {fileExtension} не поддерживается в качестве аватарки пользователя");
        var user = await booklyDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user == null) return Result<string>.Failure("Пользователь с указанным Id не найден");
        var res = await filesService.UploadFileAsync(request.File.File);
        if (res.IsFailure)
        {
            logger.Error("Произошли ошибки при загрузке аватарки: {@error}", res.Error);
            return Result<string>.Failure("Ошибка загрузки аватарки");
        }
        user.SetAvatarKey(res.Value.Key);
        await booklyDbContext.SaveChangesAsync(cancellationToken);
        return Result<string>.Success(res.Value.PresignedUrl);
    }
}

public record UpdateUserAvatarCommand(Guid UserId, FileDto File) : IRequest<Result<string>>;