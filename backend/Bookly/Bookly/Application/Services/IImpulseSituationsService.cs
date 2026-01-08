using Core.Dto.ImpulseSituations;

namespace Bookly.Application.Services;

public interface IImpulseSituationsService
{
    List<ImpulseSituationShortDto> GetImpulseSituations();
    ImpulseSituationFullDto? GetImpulseSituation(string situation);
}