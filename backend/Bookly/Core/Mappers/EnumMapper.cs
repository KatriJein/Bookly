using Core.Enums;

namespace Core.Mappers;

public static class EnumMapper
{
    public static string MapAgeRestrictionEnumToString(AgeRestriction ageRestriction)
    {
        return ageRestriction switch
        {
            AgeRestriction.Everyone => "0+",
            AgeRestriction.Children => "6+",
            AgeRestriction.Teen => "12+",
            AgeRestriction.YoungAdult => "16+",
            AgeRestriction.Mature => "18+",
            AgeRestriction.Unspecified => "Отсутствует возрастное ограничение",
            _ => throw new NotImplementedException("Неизвестное значение AgeRestriction")
        };
    }
}