// GoalSettings.cs
// Represents the user's goal for weight management.

namespace MacroNutrientCalc.Models
{
    public class GoalSettings
    {
        public string? Type { get; set; }           // User's goal for weight management, i.e., "Maintain", "Lose Weight", or "Gain Weight"
        public string? Rate { get; set; }           // Rate of weight change (e.g., 0.5, 1, 1.5, 2 lbs per week)
        public string MacroRatio { get; set; } = "Standard";    // Macronutrient ratio desired, i.e., "Standard" (Standard American Diet), "Keto" (Ketogenic Diet), "Zone" (Zone Diet)
    }
}
