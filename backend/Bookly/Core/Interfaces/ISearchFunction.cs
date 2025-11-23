namespace Core.Interfaces;

public interface ISearchFunction<T>
{
    IQueryable<T> Apply(IQueryable<T> items);
}