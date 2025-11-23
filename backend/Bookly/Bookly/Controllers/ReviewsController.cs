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
    /// Создать свой новый отзыв
    /// </summary>
    [HttpPost]
    [Authorize]
    [Route("")]
    public async Task<IActionResult> Create([FromBody] CreateReviewDto createReviewDto)
    {
        var res = await mediator.Send(new CreateReviewCommand(createReviewDto, User.RetrieveUserId()));
        return res.IsSuccess
            ? Created("/api/reviews", res.Value)
            : BadRequest(res.Error);
    }

    /// <summary>
    /// Обновить свой отзыв
    /// </summary>
    [HttpPut]
    [Authorize]
    [Route("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateReviewDto updateReviewDto)
    {
        var res = await mediator.Send(new UpdateReviewCommand(updateReviewDto, id, User.RetrieveUserId()));
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