using Bookly.Domain.Models;
using Core.Enums;
using Core.Interfaces;

namespace Bookly.Application.Functions.Books;

public class ByVolumeSizeSearchFunction(VolumeSizePreference? volumeSizePreference) : ISearchFunction<Book>
{
    public IQueryable<Book> Apply(IQueryable<Book> items)
    {
        if (volumeSizePreference is null) return items;
        return volumeSizePreference switch
        {
            VolumeSizePreference.Short => items.Where(b => b.PageCount < 200),
            VolumeSizePreference.Medium => items.Where(b => b.PageCount >= 200 && b.PageCount < 500),
            VolumeSizePreference.Long => items.Where(b => b.PageCount >= 500 && b.PageCount < 800),
            VolumeSizePreference.VeryLong => items.Where(b => b.PageCount >= 800),
            _ => items
        };
    }
}