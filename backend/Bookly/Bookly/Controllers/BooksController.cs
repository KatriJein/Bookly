using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bookly.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Получить все книги
    /// </summary>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAllBooks(CancellationToken cancellationToken)
    {
        return Ok();
    }
}