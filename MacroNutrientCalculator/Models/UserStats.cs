/// <sumary>
/// Represents the user's physical attributes and activity level.
/// </sumary>

namespace MacroNutrientCalc.Models
{
    public class UserStats
    {
        public double? Weight { get; set; }      // Weight in pounds (lbs)
        public double? Height { get; set; }      // Height in inches
        public int? Age { get; set; }            // Age in years
        public string? Sex { get; set; }         // "Male" or "Female"
        public string? ActivityLevel { get; set; }  // Activity level, e.g., "Sedentary", "Moderately Active", etc...
    }
}
