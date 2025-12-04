using Bookly.Application.Handlers.Reviews;
using Bookly.Extensions;
using Core.Dto.Review;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookly.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Создать свой новый отзыв (устаревший метод, без оценки)
    /// </summary>
    [HttpPost]
    [Authorize]
    [Route("create/v1")]
    public async Task<IActionResult> CreateOld([FromBody] CreateReviewDto createReviewDto)
    {
        var res = await mediator.Send(new CreateReviewCommand(createReviewDto, User.RetrieveUserId()));
        return res.IsSuccess
            ? Created("/api/reviews", res.Value)
            : BadRequest(res.Error);
    }
    
    /// <summary>
    /// Создать свой новый отзыв (актуальный метод с указанием оценки сразу же)
    /// </summary>
    [HttpPost]
    [Authorize]
    [Route("create/v2")]
    public async Task<IActionResult> CreateNew([FromBody] CreateReviewWithRatingDto createReviewDto)
    {
        var res = await mediator.Send(new CreateReviewTransactionCommand(createReviewDto, User.RetrieveUserId()));
        return res.IsSuccess
            ? Created("/api/reviews", res.Value)
            : BadRequest(res.Error);
    }

    /// <summary>
    /// Обновить свой отзыв (устаревший метод, без оценки)
    /// </summary>
    [HttpPut]
    [Authorize]
    [Route("{id:guid}/v1")]
    public async Task<IActionResult> UpdateOld([FromRoute] Guid id, [FromBody] UpdateReviewDto updateReviewDto)
    {
        var res = await mediator.Send(new UpdateReviewCommand(updateReviewDto, id, User.RetrieveUserId()));
        return res.IsSuccess
            ? NoContent()
            : BadRequest(res.Error);
    }
    
    /// <summary>
    /// Обновить свой отзыв (актуальный метод)
    /// </summary>
    [HttpPut]
    [Authorize]
    [Route("{id:guid}/v2")]
    public async Task<IActionResult> UpdateNew([FromRoute] Guid id, [FromBody] UpdateReviewWithRatingDto updateReviewDto)
    {
        var res = await mediator.Send(new UpdateReviewTransactionCommand(updateReviewDto, id, User.RetrieveUserId()));
        return res.IsSuccess
            ? NoContent()
            : BadRequest(res.Error);
    }
    
    /// <summary>
    /// Удалить свой отзыв
    /// </summary>
    [HttpDelete]
    [Authorize]
    [Route("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var res = await mediator.Send(new RemoveReviewCommand(id, User.RetrieveUserId()));
        return res.IsSuccess
            ? NoContent()
            : BadRequest(res.Error);
    }
}