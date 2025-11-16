using Bookly.Application.Handlers.BookCollections;
using Bookly.Application.Handlers.Ratings;
using Bookly.Domain.Models;
using Bookly.Extensions;
using Core.Dto.Book;
using Core.Dto.BookCollection;
using Core.Dto.Rating;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookly.Controllers;

[ApiController]
[Route("api/book-collections")]
public class BookCollectionsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Получить все публичные подборки, начиная с наиболее популярных
    /// </summary>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAll([FromQuery] BookCollectionSearchSettingsDto bookCollectionSearchSettingsDto, CancellationToken cancellationToken)
    {
        var collections = await mediator.Send(new GetBookCollectionsQuery(bookCollectionSearchSettingsDto, null), cancellationToken);
        return Ok(collections);
    }
    
    /// <summary>
    /// Получить все подборки пользователя, начиная со статичных
    /// </summary>
    [HttpGet]
    [Route("{userId:guid}")]
    public async Task<IActionResult> GetAll([FromRoute] Guid userId, [FromQuery] BookCollectionSearchSettingsDto bookCollectionSearchSettingsDto,
        CancellationToken cancellationToken)
    {
        var collections = await mediator.Send(new GetBookCollectionsQuery(bookCollectionSearchSettingsDto, userId), cancellationToken);
        return Ok(collections);
    }

    /// <summary>
    /// Получить список названий своих подборок, начиная со статичных
    /// </summary>
    [HttpGet]
    [Authorize]
    [Route("brief")]
    public async Task<IActionResult> GetAllBrief(CancellationToken cancellationToken)
    {
        var collections = await mediator.Send(new GetBookCollectionsBriefInfoQuery(User.RetrieveUserId()), cancellationToken);
        return Ok(collections);
    }

    /// <summary>
    /// Создать новую коллекцию у пользователя
    /// </summary>
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Create([FromBody] CreateBookCollectionDto createBookCollectionDto)
    {
        var res = await mediator.Send(new CreateBookCollectionCommand(createBookCollectionDto));
        return res.IsSuccess
            ? Created("/api/book-collections", res.Value)
            : BadRequest(res.Error);
    }
    
    /// <summary>
    /// Оценить коллекцию
    /// </summary>
    [HttpPost]
    [Route("{id:guid}/rate")]
    public async Task<IActionResult> Rate([FromRoute] Guid id, [FromBody] RatingDto ratingDto)
    {
        var res = await mediator.Send(
            new AddOrUpdateRatingCommand<BookCollection>(db => db.BookCollections, id, User.RetrieveUserId(), ratingDto.Rating));
        return res.IsSuccess ? NoContent() : BadRequest(res.Error);
    }

    /// <summary>
    /// Добавить книгу в коллекции
    /// </summary>
    [HttpPost]
    [Authorize]
    [Route("add")]
    public async Task<IActionResult> AddBookToCollections([FromBody] AddBookToCollectionsDto addBookToCollectionsDto)
    {
        var res = await mediator.Send(new AddBookToBookCollectionsCommand(addBookToCollectionsDto.CollectionIds,
            addBookToCollectionsDto.BookId, User.RetrieveUserId()));
        return res.IsFailure ? BadRequest(res.Error) : NoContent();
    }
    
    /// <summary>
    /// Добавить книгу в статичную коллекцию
    /// </summary>
    [HttpPost]
    [Authorize]
    [Route("static/add")]
    public async Task<IActionResult> AddBookToStatic([FromBody] AddBookToStaticCollectionDto addBookToStaticCollectionDto)
    {
        var res = await mediator.Send(new AddBookToStaticCollectionCommand(addBookToStaticCollectionDto.CollectionName,
            addBookToStaticCollectionDto.BookId, User.RetrieveUserId()));
        return res.IsFailure ? BadRequest(res.Error) : NoContent();
    }
    
    /// <summary>
    /// Удалить книгу из коллекции
    /// </summary>
    [HttpPost]
    [Authorize]
    [Route("remove")]
    public async Task<IActionResult> RemoveBookFromCollection([FromBody] RemoveBookFromCollectionDto removeBookFromCollectionDto)
    {
        var res = await mediator.Send(new RemoveBookFromBookCollectionCommand(removeBookFromCollectionDto.CollectionId,
            removeBookFromCollectionDto.BookId, User.RetrieveUserId()));
        return res.IsFailure ? BadRequest(res.Error) : NoContent();
    }
    
    /// <summary>
    /// Удалить книгу из статичной коллекции
    /// </summary>
    [HttpPost]
    [Authorize]
    [Route("static/remove")]
    public async Task<IActionResult> RemoveBookFromStaticCollection([FromBody] RemoveBookFromStaticCollectionDto removeBookFromStaticCollectionDto)
    {
        var res = await mediator.Send(new RemoveBookFromStaticCollectionCommand(removeBookFromStaticCollectionDto.CollectionName,
            removeBookFromStaticCollectionDto.BookId, User.RetrieveUserId()));
        return res.IsFailure ? BadRequest(res.Error) : NoContent();
    }
    
    /// <summary>
    /// Обновить свою динамическую коллекцию
    /// </summary>
    [HttpPut]
    [Authorize]
    [Route("{id:guid}")]
    public async Task<IActionResult> Update([FromBody] UpdateBookCollectionDto bookCollectionDto, [FromRoute] Guid id)
    {
        var res = await mediator.Send(new UpdateBookCollectionCommand(bookCollectionDto, id, User.RetrieveUserId()));
        return res.IsFailure ? BadRequest(res.Error) : NoContent();
    }
    
    /// <summary>
    /// Удалить свою динамическую коллекцию
    /// </summary>
    [HttpDelete]
    [Authorize]
    [Route("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var res = await mediator.Send(new DeleteBookCollectionCommand(id, User.RetrieveUserId()));
        return res.IsFailure ? BadRequest(res.Error) : NoContent();
    }
}