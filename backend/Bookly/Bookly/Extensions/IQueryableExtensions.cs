namespace Bookly.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<T> RetrieveNextPage<T>(this IQueryable<T> items, int page, int size) where T : class
    {
        return items.Skip((page - 1) * size).Take(size);
    }
}