using Core;

namespace Bookly.Domain.Models;

public class ScrapingTaskState : Entity<Guid>
{
    public int CurrentGenreIndex { get; private set; }
    public int CurrentPublisherIndex { get; private set; }
    public int CurrentIndex { get; private set; }
    public DateTime CreatedAt  { get; private set; }
    public DateTime? CooldownUntil { get; private set; }

    public static ScrapingTaskState Create(int currentGenreIndex, int currentPublisherIndex, int currentIndex,
        DateTime? cooldownUntil)
    {
        return new ScrapingTaskState()
        {
            CurrentGenreIndex = currentGenreIndex,
            CreatedAt = DateTime.UtcNow,
            CooldownUntil = cooldownUntil,
            CurrentIndex = currentIndex,
            CurrentPublisherIndex = currentPublisherIndex
        };
    }
}