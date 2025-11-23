using Microsoft.AspNetCore.Http;

namespace Core.Dto.File;

public record FileDto(IFormFile File);