using Bookly.Application.Handlers.Genres;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bookly.Controllers;

[ApiController]
[Route("api/genres")]
public class GenresController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Получить все существующие жанры
    /// </summary>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAllGenres(CancellationToken cancellationToken)
    {
        var genres = await mediator.Send(new GetAllGenresQuery(), cancellationToken);
        return Ok(genres);
    }
}