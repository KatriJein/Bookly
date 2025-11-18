using Bookly.Application.Handlers.Reviews;
using Bookly.Application.Handlers.Users;
using Bookly.Extensions;
using Core.Dto.File;
using Core.Dto.Review;
using Core.Dto.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookly.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Получить всех пользователей (без подключения presignedUrl аватарки, остается ключ-название файла)
    /// </summary>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var users = await mediator.Send(new GetUsersQuery(), cancellationToken);
        return Ok(users);
    }
    
    /// <summary>
    /// Получить полную информацию
    /// </summary>
    [HttpGet]
    [Route("{id:guid}/full")]
    public async Task<IActionResult> GetFullInfo([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var user = await mediator.Send(new GetFullUserQuery(id), cancellationToken);
        if (user is null) return NotFound();
        return Ok(user);
    }
    
    /// <summary>
    /// Получить краткую информацию
    /// </summary>
    [HttpGet]
    [Route("{id:guid}/short")]
    public async Task<IActionResult> GetShortInfo([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var user = await mediator.Send(new GetFullUserQuery(id), cancellationToken);
        return Ok(user);
    }
    
    /// <summary>
    /// Получить все свои отзывы
    /// </summary>
    [HttpGet]
    [Authorize]
    [Route("reviews")]
    public async Task<IActionResult> GetReviews([FromQuery] ReviewSearchSettingsDto reviewSearchSettingsDto, CancellationToken cancellationToken)
    {
        var reviews = await mediator.Send(new GetReviewsQuery(reviewSearchSettingsDto, null, User.RetrieveUserId()), cancellationToken);
        return Ok(reviews);
    }
    
    /// <summary>
    /// Обновить основную информацию
    /// </summary>
    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateMainInfo([FromRoute] Guid id, [FromBody] UpdateUserDto updateUserDto)
    {
        var res = await mediator.Send(new UpdateUserCommand(id, updateUserDto));
        if (res.IsFailure) return BadRequest(res.Error);
        return NoContent();
    }
    
    /// <summary>
    /// Обновить пароль
    /// </summary>
    [HttpPatch]
    [Route("{id:guid}/password")]
    public async Task<IActionResult> UpdatePassword([FromRoute] Guid id, [FromBody] UpdatePasswordDto updatePasswordDto)
    {
        var res = await mediator.Send(new UpdateUserPasswordCommand(id, updatePasswordDto));
        if (res.IsFailure) return BadRequest(res.Error);
        return NoContent();
    }
    
    /// <summary>
    /// Обновить аватарку
    /// </summary>
    [HttpPatch]
    [Route("{id:guid}/avatar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadAvatar([FromForm] FileDto fileDto, [FromRoute] Guid id)
    {
        var res = await mediator.Send(new UpdateUserAvatarCommand(id, fileDto));
        if (res.IsFailure) return BadRequest(res.Error);
        return Ok(res.Value);
    }

    /// <summary>
    /// Удалить пользователя
    /// </summary>
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }
}