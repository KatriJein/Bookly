using Bookly.Domain.Models;
using Core;
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
            VolumeSizePreference.Short => items.Where(b => b.PageCount < Const.ShortBookMaxPagesCount),
            VolumeSizePreference.Medium => items.Where(b => b.PageCount >= Const.ShortBookMaxPagesCount && b.PageCount < Const.MediumBookMaxPagesCount),
            VolumeSizePreference.Long => items.Where(b => b.PageCount >= Const.MediumBookMaxPagesCount && b.PageCount < Const.LongBookMaxPagesCount),
            VolumeSizePreference.VeryLong => items.Where(b => b.PageCount >= Const.LongBookMaxPagesCount),
            _ => items
        };
    }
}