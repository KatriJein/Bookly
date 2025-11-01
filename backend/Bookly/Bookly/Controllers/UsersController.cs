using Bookly.Application.Handlers.Users;
using Core.Dto.File;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bookly.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Обновить аватарку пользователя
    /// </summary>
    [HttpPatch]
    [Route("{id:guid}/avatar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadAvatar([FromForm] FileDto fileDto, [FromRoute] Guid id)
    {
        var res = await mediator.Send(new UpdateUserAvatarCommand(fileDto));
        if (res.IsFailure) return BadRequest(res.Error);
        return Ok(res.Value);
    }
}