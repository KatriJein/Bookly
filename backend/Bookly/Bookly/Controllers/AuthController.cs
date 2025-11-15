using Bookly.Application.Handlers.Auth;
using Core.Dto.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookly.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Выполнить аутентификацию и авторизацию
    /// </summary>
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> AuthenthicateAsync([FromBody] AuthRequestDto request)
    {
        var authRes = await mediator.Send(new AuthenthicationCommand(request));
        if (authRes.IsFailure) return BadRequest(authRes.Error);
        return Ok(authRes.Value);
    }

    /// <summary>
    /// Проверить аутентификацию пользователя
    /// </summary>
    [HttpPost]
    [Route("am-authenticated")]
    [Authorize]
    public IActionResult CheckAuthentication()
    {
        return Ok();
    }

    /// <summary>
    /// Зарегистрировать нового пользователя
    /// </summary>
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegistrationRequestDto request)
    {
        var registrationRes = await mediator.Send(new RegistrationCommand(request));
        if (registrationRes.IsFailure) return BadRequest(registrationRes.Error);
        return Ok(registrationRes.Value);
    }
}