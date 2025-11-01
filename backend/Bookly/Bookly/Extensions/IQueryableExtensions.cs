using Core.Interfaces;

namespace Bookly.Extensions;

public static class IQueryableExtensions
{
    public static IQueryable<T> RetrieveNextPage<T>(this IQueryable<T> items, int page, int size) where T : class
    {
        return items.Skip((page - 1) * size).Take(size);
    }

    public static IQueryable<T> ApplySearchFunctionsForItems<T>(this IQueryable<T> items,
        List<ISearchFunction<T>> searchFunctions) where T : class
    {
        searchFunctions.ForEach(f => items = f.Apply(items));
        return items;
    }

    public static IQueryable<T> ApplySortingForItems<T>(this IQueryable<T> items, ISortingFunction<T> sortingFunction) where T : class
    {
        return sortingFunction.Apply(items);
    }
}