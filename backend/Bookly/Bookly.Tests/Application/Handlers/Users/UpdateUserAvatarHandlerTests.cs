using Bookly.Application.Handlers.Users;
using Bookly.Application.Services.Files;
using Bookly.Domain.Models;
using Bookly.Infrastructure;
using Bookly.Tests.Utils;
using Core;
using Core.Dto.File;
using Core.Dto.User;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Bookly.Tests.Application.Handlers.Users;

[TestFixture]
public class UpdateUserAvatarHandlerTests
{
    private BooklyDbContext _db = null!;
    private IFilesService _filesService = null!;
    private Serilog.ILogger _logger = null!;

    [SetUp]
    public void Setup()
    {
        _db = DatabaseUtils.CreateDbContext();
        _filesService = Substitute.For<IFilesService>();
        _logger = Substitute.For<Serilog.ILogger>();
    }

    [TearDown]
    public void Teardown()
    {
        _db.Dispose();
    }

    private static IFormFile CreateFormFile(string name = "avatar.jpg", string contentType = "image/jpeg")
    {
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        return new FormFile(stream, 0, stream.Length, "file", name) { Headers = new HeaderDictionary(), ContentType = contentType };
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenExtensionNotSupported()
    {
        // Arrange
        var file = new FileDto(CreateFormFile("avatar.bmp", "image/bmp"));
        var handler = new UpdateUserAvatarHandler(_db, _filesService, _logger);
        var command = new UpdateUserAvatarCommand(Guid.NewGuid(), file);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Does.Contain(".bmp"));
        await _filesService.DidNotReceiveWithAnyArgs().UploadFileAsync(default!);
    }

    [Test]
    public async Task Handle_ReturnsFailure_WhenUserNotFound()
    {
        // Arrange
        var file = new FileDto(CreateFormFile("avatar.jpg"));
        var handler = new UpdateUserAvatarHandler(_db, _filesService, _logger);
        var command = new UpdateUserAvatarCommand(Guid.NewGuid(), file);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Does.Contain("Пользователь"));
        await _filesService.DidNotReceiveWithAnyArgs().UploadFileAsync(default!);
    }

    [Test]
    public async Task Handle_LogsError_WhenFileUploadFails()
    {
        // Arrange
        var user = User.Create(new CreateUserDto("user", "mail@mail.com", "hash")).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var file = new FileDto(CreateFormFile("avatar.jpg"));
        _filesService.UploadFileAsync(file.File).Returns(Result<UploadedFileDto>.Failure("ошибка загрузки"));

        var handler = new UpdateUserAvatarHandler(_db, _filesService, _logger);
        var command = new UpdateUserAvatarCommand(user.Id, file);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var updated = await _db.Users.FirstAsync();
        Assert.That(result.IsFailure, Is.True);
        Assert.That(updated.AvatarKey, Is.Null.Or.Empty);
    }

    [Test]
    public async Task Handle_UpdatesUserAvatar_WhenUploadSucceeds()
    {
        // Arrange
        var user = User.Create(new CreateUserDto("user2", "mail2@mail.com", "hash")).Value;
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var file = new FileDto(CreateFormFile("cool.png"));
        var uploaded = new UploadedFileDto("avatar_key", "https://cdn/avatar.png");
        _filesService.UploadFileAsync(file.File).Returns(Result<UploadedFileDto>.Success(uploaded));

        var handler = new UpdateUserAvatarHandler(_db, _filesService, _logger);
        var command = new UpdateUserAvatarCommand(user.Id, file);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("https://cdn/avatar.png"));

        var savedUser = await _db.Users.FirstAsync();
        Assert.That(savedUser.AvatarKey, Is.EqualTo("avatar_key"));

        await _filesService.Received(1).UploadFileAsync(file.File);
    }
}