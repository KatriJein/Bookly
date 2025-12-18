using Core.Enums;

namespace Core.Data;

public static class SimilarityScores
{
    public static Dictionary<BookSimilarityType, double> Scores = new()
    {
        { BookSimilarityType.ByAgeRestriction, 0.5 },
        { BookSimilarityType.ByGenre, 2 },
        { BookSimilarityType.ByAuthor, 2 },
        { BookSimilarityType.ByLanguage, 0.5 },
        { BookSimilarityType.ByVolumeSize, 0.5 },
        { BookSimilarityType.ByReadBooks, 1 }
    };
    
    public static double MaxPossibleScoreForSimilarBooksHandler => Scores[BookSimilarityType.ByAgeRestriction]
                                             + Scores[BookSimilarityType.ByAuthor]
                                             + Scores[BookSimilarityType.ByLanguage]
                                             + Scores[BookSimilarityType.ByVolumeSize]
                                             + Scores[BookSimilarityType.ByGenre];
    
    public static double MaxPossibleScoreForPossiblyLikedBooksHandler => Scores[BookSimilarityType.ByAgeRestriction] 
                                                                         +  Scores[BookSimilarityType.ByGenre]
                                                                         +  Scores[BookSimilarityType.ByAuthor]
                                                                         +  Scores[BookSimilarityType.ByVolumeSize]
                                                                         +  Scores[BookSimilarityType.ByReadBooks];
    
    
    public static double MaxPossibleScoreForUserInterestBooksHandler => Scores[BookSimilarityType.ByAgeRestriction]
                                            + Scores[BookSimilarityType.ByAuthor]
                                            + Scores[BookSimilarityType.ByGenre]
                                            + Scores[BookSimilarityType.ByVolumeSize];
}