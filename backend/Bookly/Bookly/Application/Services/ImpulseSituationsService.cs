using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Dto.ImpulseSituations;

namespace Bookly.Application.Services;

public class ImpulseSituationsService(ILogger<ImpulseSituationsService> logger, IWebHostEnvironment environment)
    : IImpulseSituationsService, IInitializableSingleton
{
    private readonly Dictionary<string, ImpulseSituationFullDto> _impulseSituationsFull = new();
    private readonly List<ImpulseSituationShortDto> _impulseSituationsShort = [];
    
    private const string ImpulseSituationsFileName = "impulseSituations.json";
    
    public async Task InitializeAsync()
    {
        var impulseSituationsFilePath = Path.Combine(environment.ContentRootPath,  ImpulseSituationsFileName);
        Console.WriteLine(impulseSituationsFilePath);
        var fileExists = File.Exists(impulseSituationsFilePath);
        if (!fileExists)
        {
            logger.LogWarning("Отсутствует json файл impulseSituations.json");
            return;
        }
        await using var fileStream = new FileStream(impulseSituationsFilePath, FileMode.Open);
        List<ImpulseSituationFullDto>? impulseSituations;
        try
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                Converters = { new JsonStringEnumConverter() }
            };
            impulseSituations = await JsonSerializer.DeserializeAsync<List<ImpulseSituationFullDto>>(fileStream, jsonOptions);
        }
        catch (JsonException e)
        {
            logger.LogError(e, "Произошла ошибка при извлечении данных из файла impulseSituations.json");
            return;
        }
        if (impulseSituations is null)
        {
            logger.LogWarning("Пустые данные в файле impulseSituations.json");
            return;
        }
        foreach (var impulseSituation in impulseSituations)
        {
            _impulseSituationsFull[impulseSituation.Situation] = impulseSituation;
            _impulseSituationsShort.Add(new ImpulseSituationShortDto(impulseSituation.Situation, impulseSituation.Note));
        }
        logger.LogInformation("Ситуации из файла impulseSituations.json успешно извлечены в память");
    }

    public List<ImpulseSituationShortDto> GetImpulseSituations()
    {
        return _impulseSituationsShort;
    }

    public ImpulseSituationFullDto? GetImpulseSituation(string situation)
    {
        _impulseSituationsFull.TryGetValue(situation, out var impulseSituation);
        return impulseSituation;
    }
}