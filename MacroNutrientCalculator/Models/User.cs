// Models for WeightTracker.razor
using System.ComponentModel.DataAnnotations;

namespace MacroNutrientCalc.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;    // Plaintext for simplicity; use hashing for real applications
    }

    public class WeightLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timpestamp { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Weight is required.")]
        [Range(1, 1000, ErrorMessage = "Weight must be between 1 and 1000 lbs.")]
        public double? Weight { get; set; }  // in pounds

        [Required(ErrorMessage = "Height is required.")]
        [Range(36, 120, ErrorMessage = "Height must be between 36 and 120 inches.")]
        public double? Height { get; set; } // in inches

        [Range(0, 100, ErrorMessage = "Body fat percentage must bee between 0 and 100.")]
        public double? BodyFatPercentage { get; set; }  // Optional

        // Calculated properties
        public double BMI => (Weight.HasValue && Height.HasValue) ? (Weight.Value / (Height.Value * Height.Value)) * 703 : 0;
        public double? BodyFatWeight => BodyFatPercentage.HasValue ? Weight * (BodyFatPercentage.Value / 100) : 0;
        public double? LeanMass => BodyFatWeight.HasValue ? Weight - BodyFatWeight : null;
    }
}
