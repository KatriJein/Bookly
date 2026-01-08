using Bookly.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bookly.Controllers;

[ApiController]
[Route("api/impulse-situations")]
public class ImpulseSituationsController(IImpulseSituationsService impulseSituationsService) : ControllerBase
{
    /// <summary>
    /// Получить список ситуаций для фичи "Книга-Импульс"
    /// </summary>
    [HttpGet]
    [Route("")]
    public IActionResult GetAll()
    {
        var situations = impulseSituationsService.GetImpulseSituations();
        return Ok(situations);
    }
}