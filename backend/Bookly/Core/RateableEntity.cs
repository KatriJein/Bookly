using System.ComponentModel.DataAnnotations.Schema;
using Core.Interfaces;

namespace Core;

public abstract class RateableEntity : Entity<Guid>, IRateable
{
    public double Rating { get; protected set; }
    public int RatingsCount { get; protected set; }
    public void AddNewRating(int newValue)
    {
        RatingsCount++;
        Rating = (Rating * (RatingsCount - 1) + newValue) / RatingsCount;
    }
    public void RefreshRating(int oldValue, int newValue)
    {
        if (RatingsCount == 0) return;
        Rating = (Rating * RatingsCount - oldValue + newValue) / RatingsCount;
    }
    
    [NotMapped]
    public int? UserRating { get; set; }
}