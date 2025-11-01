using Bookly.Application.Services.Files;
using Core;
using Core.Dto.File;
using MediatR;

namespace Bookly.Application.Handlers.Users;

public class UpdateUserAvatarHandler(IFilesService filesService) : IRequestHandler<UpdateUserAvatarCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateUserAvatarCommand request, CancellationToken cancellationToken)
    {
        var fileExtension = Path.GetExtension(request.File.File.FileName);
        if (!Const.SupportedAvatarFileExtensions.Contains(fileExtension))
            return Result<string>.Failure($"Формат файла {fileExtension} не поддерживается в качестве аватарки пользователя");
        var res = await filesService.UploadFileAsync(request.File.File);
        return res;
    }
}

public record UpdateUserAvatarCommand(FileDto File) : IRequest<Result<string>>;