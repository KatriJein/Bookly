using Bookly.Application.Handlers.Files;
using Bookly.Application.Services.Files;
using Core;
using Core.Dto.File;
using NSubstitute;

namespace Bookly.Tests.Application.Handlers.Files;

[TestFixture]
public class GetPresignedUrlHandlerTests
{
    private IFilesService _filesService = null!;

    [SetUp]
    public void Setup()
    {
        _filesService = Substitute.For<IFilesService>();
    }

    [Test]
    public async Task Handle_ReturnsUrl_WhenServiceSuccess()
    {
        // Arrange
        var dto = new GetObjectPresinedUrlDto("bucket", "file_key");
        var expectedUrl = "https://s3.url/file_key";
        _filesService.GetPresignedUrlAsync(dto.Bucket, dto.Key)
            .Returns(Result<string>.Success(expectedUrl));

        var handler = new GetPresignedUrlHandler(_filesService);

        // Act
        var result = await handler.Handle(new GetPresignedUrlQuery(dto), CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedUrl));
    }

    [Test]
    public async Task Handle_ReturnsEmptyString_WhenKeyIsNull()
    {
        // Arrange
        var dto = new GetObjectPresinedUrlDto("bucket", null);
        var handler = new GetPresignedUrlHandler(_filesService);

        // Act
        var result = await handler.Handle(new GetPresignedUrlQuery(dto), CancellationToken.None);

        // Assert
        Assert.That(result, Is.Empty);
        await _filesService.DidNotReceiveWithAnyArgs().GetPresignedUrlAsync(default!, default!);
    }

    [Test]
    public async Task Handle_ReturnsEmptyString_WhenServiceFails()
    {
        // Arrange
        var dto = new GetObjectPresinedUrlDto("bucket", "missing_key");
        _filesService.GetPresignedUrlAsync(dto.Bucket, dto.Key)
            .Returns(Result<string>.Failure("ошибка"));
        var handler = new GetPresignedUrlHandler(_filesService);

        // Act
        var result = await handler.Handle(new GetPresignedUrlQuery(dto), CancellationToken.None);

        // Assert
        Assert.That(result, Is.Empty);
    }
}