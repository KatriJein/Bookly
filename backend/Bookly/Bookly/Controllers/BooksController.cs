using Bookly.Application.Handlers.Books;
using Bookly.Extensions;
using Core.Dto.Book;
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
    public async Task<IActionResult> GetAllBooks([FromQuery] BookSearchSettingsDto bookSearchSettingsDto, CancellationToken cancellationToken)
    {
        var books = await mediator.Send(new GetAllBooksQuery(bookSearchSettingsDto, User.RetrieveUserId()), cancellationToken);
        return Ok(books);
    }

    /// <summary>
    /// Получить полную информацию об отдельной книге
    /// </summary>
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetBook([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var book = await mediator.Send(new GetBookQuery(id, User.RetrieveUserId()), cancellationToken);
        if (book == null) return NotFound();
        return Ok(book);
    }
    
    /// <summary>
    /// Добавить новую книгу
    /// </summary>
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Create([FromBody] CreateBookDto createBookDto)
    {
        var res = await mediator.Send(new CreateBookCommand(createBookDto));
        if (res.IsFailure) return BadRequest(res.Error);
        return Created("/books", res.Value);
    }

    /// <summary>
    /// Удалить книгу
    /// </summary>
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var res = await mediator.Send(new DeleteBookCommand(id));
        return NoContent();
    }
}