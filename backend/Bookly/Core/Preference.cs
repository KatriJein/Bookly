using Core.Enums;
using Core.Interfaces;

namespace Core;

public abstract class Preference : Entity<Guid>, IPreference
{
    public PreferenceType PreferenceType { get; set; }
    public double Weight { get; set; }

    private const double PreferenceSide = 0.3;
    private const double MaxWeight = 1;
    
    public void SetWeight(double weight)
    {
        if (weight > MaxWeight) Weight = MaxWeight;
        else if (weight < -MaxWeight) Weight = -MaxWeight;
        else Weight = weight;
        UpdatePreferenceType();
    }

    public void UpdatePreferenceType()
    {
        if (Weight >= PreferenceSide) PreferenceType = PreferenceType.Liked;
        else if (Weight <= -PreferenceSide) PreferenceType = PreferenceType.Disliked;
        else PreferenceType = PreferenceType.Neutral;
    }
}