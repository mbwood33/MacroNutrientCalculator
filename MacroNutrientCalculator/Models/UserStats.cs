/// <sumary>
/// Represents the user's physical attributes and activity level.
/// </sumary>
using System.ComponentModel.DataAnnotations;

namespace MacroNutrientCalc.Models
{
    public class UserStats
    {
        [Required(ErrorMessage = "Weight is required.")]
        public double? Weight { get; set; }      // Weight in pounds (lbs)

        [Required(ErrorMessage = "Height is required.")]
        public double? Height { get; set; }      // Height in inches

        [Required(ErrorMessage = "Age is required.")]
        public int? Age { get; set; }            // Age in years

        [Required(ErrorMessage = "Sex is required.")]
        public string? Sex { get; set; }         // "Male" or "Female"

        [Required(ErrorMessage = "Activity Level is required.")]
        public string? ActivityLevel { get; set; }  // Activity level, e.g., "Sedentary", "Moderately Active", etc...
    }
}
