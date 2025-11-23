namespace Core.Interfaces;

public interface ISortingFunction<T>
{
    IQueryable<T> Apply(IQueryable<T> items);
}