using Bookly.Application.Handlers.Publishers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bookly.Controllers;

[ApiController]
[Route("api/publishers")]
public class PublishersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Получить всех издателей
    /// </summary>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var publishers = await mediator.Send(new GetPublishersQuery(), cancellationToken);
        return Ok(publishers);
    }
}