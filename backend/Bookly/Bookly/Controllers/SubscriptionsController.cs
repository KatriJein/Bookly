using Bookly.Application.Handlers.UserSubscriptions;
using Bookly.Extensions;
using Core.Dto.UserSubscription;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookly.Controllers;

[ApiController]
[Route("api/subscriptions")]
public class SubscriptionsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Получить все подписки пользователя
    /// </summary>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAllUserSubscriptions(CancellationToken cancellationToken)
    {
        var subscriptions = await mediator.Send(new GetUserSubscriptionsQuery(User.RetrieveUserId()),
            cancellationToken);
        return Ok(subscriptions);
    }
    
    /// <summary>
    /// Оформить подписку на пользователя (метод требует аутентификации)
    /// </summary>
    [HttpPost]
    [Route("")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateUserSubscriptionDto createUserSubscriptionDto)
    {
        var result = await mediator.Send(new SubscribeToUserCommand(User.RetrieveUserId(), createUserSubscriptionDto.FollowingId));
        return result.IsFailure ? BadRequest(result.Error) : NoContent();
    }
    
    /// <summary>
    /// Отписаться от пользователя (метод требует аутентификации)
    /// </summary>
    [HttpDelete]
    [Route("")]
    [Authorize]
    public async Task<IActionResult> Delete([FromQuery] Guid followingUserId)
    {
        var result = await mediator.Send(new UnsubscribeFromUserCommand(User.RetrieveUserId(), followingUserId));
        return result.IsFailure ? BadRequest(result.Error) : NoContent();
    }
}