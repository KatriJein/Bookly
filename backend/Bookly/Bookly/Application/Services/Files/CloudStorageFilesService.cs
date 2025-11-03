using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Core;
using Core.Dto.File;
using Core.Options;
using Microsoft.Extensions.Options;
using ILogger = Serilog.ILogger;

namespace Bookly.Application.Services.Files;

public class CloudStorageFilesService(IOptionsSnapshot<BooklyOptions> booklyOptions, ILogger logger) : IFilesService
{
    private readonly IAmazonS3 _client = new AmazonS3Client(new AmazonS3Config() {ServiceURL = booklyOptions.Value.BucketServiceUrl});
    
    public async Task<Result<UploadedFileDto>> UploadFileAsync(IFormFile file)
    {
        await _client.EnsureBucketExistsAsync(booklyOptions.Value.BooklyFilesStorageBucketName);
        var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{Path.GetExtension(file.FileName)}";
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;
        var putObjectRequest = new PutObjectRequest()
        {
            BucketName = booklyOptions.Value.BooklyFilesStorageBucketName,
            ContentType = file.ContentType,
            Key = fileName,
            InputStream = stream
        };
        await _client.PutObjectAsync(putObjectRequest);
        var presignedUrl = await GetPresignedUrlAsync(booklyOptions.Value.BooklyFilesStorageBucketName, fileName);
        return presignedUrl.IsFailure ?
            Result<UploadedFileDto>.Failure(presignedUrl.Error)
            : Result<UploadedFileDto>.Success(new UploadedFileDto(fileName, presignedUrl.Value));
    }

    public async Task<Result<string>> GetPresignedUrlAsync(string bucketName, string key)
    {
        var getPresignedUrlRequest = new GetPreSignedUrlRequest()
        {
            BucketName = bucketName,
            Expires = DateTime.UtcNow.AddDays(1),
            Key = key
        };
        var presignedUrl = await _client.GetPreSignedURLAsync(getPresignedUrlRequest);
        if (presignedUrl is not null) return Result<string>.Success(presignedUrl);
        logger.Error("Не удалось получить ссылку для объекта {@objectKey}", key);
        return Result<string>.Failure("Не удалось получить ссылку для объекта");
    }
}