using Core;
using Core.Enums;

namespace Bookly.Application;

public static class BookUtils
{
    public static VolumeSizePreference GetVolumeSizeDependingOnPagesCount(int pagesCount)
    {
        return pagesCount switch
        {
            < Const.ShortBookMaxPagesCount => VolumeSizePreference.Short,
            < Const.MediumBookMaxPagesCount => VolumeSizePreference.Medium,
            < Const.LongBookMaxPagesCount => VolumeSizePreference.Long,
            >= Const.LongBookMaxPagesCount => VolumeSizePreference.VeryLong
        };
    }
}