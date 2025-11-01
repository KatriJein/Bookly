using Core;

namespace Bookly.Application.Services.Files;

public interface IFilesService
{
    Task<Result<string>> UploadFileAsync(IFormFile file);
    Task<Result<string>> GetPresignedUrlAsync(string bucketName, string key);
}