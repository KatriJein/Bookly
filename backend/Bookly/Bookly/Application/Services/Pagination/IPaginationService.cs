namespace Bookly.Application.Services.Pagination;

public interface IPaginationService
{
    IQueryable<T> RetrieveNextPage<T>(IQueryable<T> items, int page, int size) where T : class;
}