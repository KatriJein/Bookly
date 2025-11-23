using Core.Enums;

namespace Core.Data;

public static class GenreRestrictionsData
{
    public static Dictionary<string, AgeRestriction> GenreRestrictions => new()
    {
        ["Fiction"] = AgeRestriction.Everyone,
        ["Drama"] = AgeRestriction.Everyone,
        ["Poetry"] = AgeRestriction.Everyone,
        ["Adventure"] = AgeRestriction.Children,
        ["Fantasy"] = AgeRestriction.Teen,
        ["Romance"] = AgeRestriction.YoungAdult,
        ["Science Fiction"] = AgeRestriction.Teen,
        ["Classical Literature"] = AgeRestriction.Everyone,
        ["Mystery"] = AgeRestriction.Teen,
        ["Thrillers"] = AgeRestriction.Mature,
        ["Historical Fiction"] = AgeRestriction.Teen,
        ["Biography & Autobiography"] = AgeRestriction.Everyone,
        ["Horror"] = AgeRestriction.Mature,
        ["Comedy"] = AgeRestriction.Everyone,
        ["Science"] = AgeRestriction.Everyone,
        ["Self-Help"] = AgeRestriction.YoungAdult,
        ["Psychology"] = AgeRestriction.YoungAdult,
        ["Education"] = AgeRestriction.Everyone,
        ["Juvenile Fiction"] = AgeRestriction.Children,
        ["Juvenile Nonfiction"] = AgeRestriction.Children,
        ["Nonfiction"] = AgeRestriction.YoungAdult,
        ["Young Adult"] = AgeRestriction.YoungAdult
    };
}