using Bookly.Application.Handlers.Recommendations;
using Bookly.Extensions;
using Core.Dto.Recommendation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookly.Controllers;

[ApiController]
[Route("api/recommendation-statuses")]
public class RecommendationStatusesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Сохранить ответ пользователя на предложенную рекомендацию (нравится / не нравится)
    /// </summary>
    [HttpPost]
    [Route("")]
    [Authorize]
    public async Task<IActionResult> Add([FromBody] RecommendationDto recommendationDto)
    {
        var result = await mediator.Send(new AddRecommendationStatusCommand(recommendationDto, User.RetrieveUserId()));
        return result.IsFailure
            ? BadRequest(result.Error)
            : NoContent();
    }
}