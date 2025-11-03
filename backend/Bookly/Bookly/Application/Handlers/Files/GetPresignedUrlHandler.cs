using Bookly.Application.Services.Files;
using Core.Dto.File;
using MediatR;

namespace Bookly.Application.Handlers.Files;

public class GetPresignedUrlHandler(IFilesService filesService) : IRequestHandler<GetPresignedUrlQuery, string>
{
    public async Task<string> Handle(GetPresignedUrlQuery request, CancellationToken cancellationToken)
    {
        if (request.GetObjectPresinedUrlDto.Key is null) return "";
        var presignedUrl = await filesService.GetPresignedUrlAsync(request.GetObjectPresinedUrlDto.Bucket,
            request.GetObjectPresinedUrlDto.Key);
        return presignedUrl.IsSuccess ? presignedUrl.Value : "";
    }
}

public record GetPresignedUrlQuery(GetObjectPresinedUrlDto GetObjectPresinedUrlDto) : IRequest<string>;