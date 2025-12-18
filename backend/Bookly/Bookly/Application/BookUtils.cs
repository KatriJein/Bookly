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

    public static AgeCategory GetAgeCategoryDependingOnAgeRestriction(AgeRestriction ageRestriction)
    {
        return ageRestriction switch
        {
            AgeRestriction.Everyone => AgeCategory.Baby,
            AgeRestriction.Children => AgeCategory.Child,
            AgeRestriction.Teen => AgeCategory.Teen,
            AgeRestriction.YoungAdult => AgeCategory.YoungAdult,
            AgeRestriction.Mature => AgeCategory.Adult,
            _ => AgeCategory.NotSpecified
        };
    }
}