using Bookly.Application.Handlers.Authors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bookly.Controllers;

[ApiController]
[Route("api/authors")]
public class AuthorsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Получить всех существующих авторов
    /// </summary>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var authors = await mediator.Send(new GetAllAuthorsQuery(), cancellationToken);
        return Ok(authors);
    }
}