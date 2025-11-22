namespace Core.Interfaces;

public interface IRateable
{
    Guid Id { get; }
    double Rating { get; }
    int RatingsCount { get; }
    void AddNewRating(int newValue);
    void RefreshRating(int oldValue, int newValue);
    int? UserRating { get; set; }
}