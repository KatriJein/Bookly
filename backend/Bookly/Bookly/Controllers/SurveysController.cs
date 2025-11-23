using Bookly.Application.Handlers.Survey;
using Core.Dto.Survey;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bookly.Controllers;

[ApiController]
[Route("api/surveys")]
public class SurveysController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Записать результаты входной анкеты интересов
    /// </summary>
    [HttpPost]
    [Route("entry-survey")]
    public async Task<IActionResult> SaveEntrySurvey([FromBody] EntrySurveyDataDto entrySurveyDataDto)
    {
        var result = await mediator.Send(new SaveEntrySurveyResponsesCommand(entrySurveyDataDto));
        if (result.IsFailure) return BadRequest(result.Error);
        return NoContent();
    }
}