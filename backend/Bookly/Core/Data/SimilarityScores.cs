using Core.Enums;

namespace Core.Data;

public static class SimilarityScores
{
    public static Dictionary<BookSimilarityType, double> Scores = new()
    {
        { BookSimilarityType.ByAgeRestriction, 1 },
        { BookSimilarityType.ByGenre, 2 },
        { BookSimilarityType.ByAuthor, 2 },
        { BookSimilarityType.ByLanguage, 0.5 },
        { BookSimilarityType.ByVolumeSize, 1 },
    };
}