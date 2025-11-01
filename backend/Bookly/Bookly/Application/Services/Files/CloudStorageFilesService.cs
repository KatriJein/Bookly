using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Core;
using Core.Options;
using Microsoft.Extensions.Options;
using ILogger = Serilog.ILogger;

namespace Bookly.Application.Services.Files;

public class CloudStorageFilesService(IOptionsSnapshot<BooklyOptions> booklyOptions) : IFilesService
{
    private readonly IAmazonS3 _client = new AmazonS3Client(new AmazonS3Config() {ServiceURL = booklyOptions.Value.BucketServiceUrl});
    
    public async Task<Result<string>> UploadFileAsync(IFormFile file)
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
        return await GetPresignedUrlAsync(booklyOptions.Value.BooklyFilesStorageBucketName, fileName);
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
        return presignedUrl == null 
            ? Result<string>.Failure("Не удалось получить ссылку на аватарку")
            : Result<string>.Success(presignedUrl);
    }
}