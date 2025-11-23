using Core;
using Core.Dto.File;

namespace Bookly.Application.Services.Files;

public interface IFilesService
{
    Task<Result<UploadedFileDto>> UploadFileAsync(IFormFile file);
    Task<Result<string>> GetPresignedUrlAsync(string bucketName, string key);
}