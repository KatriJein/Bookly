using Core.Data;

namespace Bookly.Application;

public static class Utils
{
    public static double CalculateJaccard<T>(HashSet<T> first, HashSet<T> second)
    {
        if (first.Count == 0 && second.Count == 0) return 1;
        var unionCount = first.Union(second).Count();
        if (unionCount == 0) return 0;
        var intersectCount = first.Intersect(second).Count();
        return (double)intersectCount / unionCount;
    }

    public static double NormalizeSimilarity(double similarityScore, double maxScore)
    {
        return similarityScore / maxScore;
    }
}