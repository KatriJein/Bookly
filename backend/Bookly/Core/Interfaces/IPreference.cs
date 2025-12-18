using Core.Enums;

namespace Core.Interfaces;

public interface IPreference
{
    PreferenceType PreferenceType { get; set; }
    double Weight { get; set; }
    void SetWeight(double weight);
    void UpdatePreferenceType();
}