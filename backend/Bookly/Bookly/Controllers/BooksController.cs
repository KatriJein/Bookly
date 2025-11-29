using Bookly.Application.Handlers.Books;
using Bookly.Application.Handlers.Ratings;
using Bookly.Application.Handlers.Recommendations;
using Bookly.Application.Handlers.Reviews;
using Bookly.Domain.Models;
using Bookly.Extensions;
using Core.Dto.Book;
using Core.Dto.Rating;
using Core.Dto.Review;
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
    /// Получить все отзывы к книге
    /// </summary>
    [HttpGet]
    [Route("{id:guid}/reviews")]
    public async Task<IActionResult> GetReviews([FromQuery] ReviewSearchSettingsDto reviewSearchSettingsDto, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var reviews = await mediator.Send(new GetReviewsQuery(reviewSearchSettingsDto, id, User.RetrieveUserId()), cancellationToken);
        return Ok(reviews);
    }

    /// <summary>
    /// Узнать, указывал ли пользователь свое мнение по рекомендации на данную книгу. Возвращает true для неавторизованного пользователя
    /// </summary>
    [HttpGet]
    [Route("{id:guid}/recommendation-status")]
    public async Task<IActionResult> HasRecommendationStatus([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new HasRecommendationResponseQuery(id, User.RetrieveUserId()), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Получить книги, похожие на данную
    /// </summary>
    [HttpGet]
    [Route("{id:guid}/similar")]
    public async Task<IActionResult> GetSimilar([FromRoute] Guid id,
        [FromQuery] BookSimpleSearchSettingsDto bookSimpleSearchSettingsDto, CancellationToken cancellationToken)
    {
        var books = await mediator.Send(new GetSimilarBooksQuery(id, bookSimpleSearchSettingsDto,
            User.RetrieveUserId()), cancellationToken);
        return Ok(books);
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
    /// Оценить книгу
    /// </summary>
    [HttpPost]
    [Route("{id:guid}/rate")]
    public async Task<IActionResult> Rate([FromRoute] Guid id, [FromBody] RatingDto ratingDto)
    {
        var res = await mediator.Send(
            new AddOrUpdateRatingCommand<Book>(db => db.Books, id, User.RetrieveUserId(), ratingDto.Rating));
        return res.IsSuccess ? NoContent() : BadRequest(res.Error);
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