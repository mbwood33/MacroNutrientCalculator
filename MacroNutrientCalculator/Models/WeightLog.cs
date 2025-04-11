// Models for WeightTracker.razor
using System;
using System.ComponentModel.DataAnnotations;

namespace MacroNutrientCalc.Models
{
    /*
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;    // Plaintext for simplicity; use hashing for real applications
    }
    */
    // Commented out 4/9/25

    public class WeightLog
    {
        public int Id { get; set; }         // Measurement record ID; use int for the record ID to match the database SERIAL type
        public int UserId { get; set; }     // Property for the foreign key linking to the user

        [Required(ErrorMessage = "Timestamp is required.")]
        public DateTime Timestamp { get; set; } = DateTime.Now;     // Date/time of measurement
        
        [Required(ErrorMessage = "Weight is required.")]
        [Range(1, 1000, ErrorMessage = "Weight must be between 1 and 1000 lbs.")]
        public double? Weight { get; set; }  // Weight in pounds
                
        [Range(36, 120, ErrorMessage = "Height must be between 36 and 120 inches.")]
        public double? Height { get; set; } // Height in inches

        [Range(0, 100, ErrorMessage = "Body fat percentage must bee between 0 and 100.")]
        public double? BodyFatPercentage { get; set; }  // Body fat percentage (Optional)

        // Calculated properties
        public double? BMI => (Weight.HasValue && Height.HasValue)
            ? (Weight.Value / (Height.Value * Height.Value)) * 703 : null;
        public double? BodyFatWeight => BodyFatPercentage.HasValue && Weight.HasValue
            ? Weight * (BodyFatPercentage.Value / 100) : null;
        public double? LeanMass => BodyFatWeight.HasValue && Weight.HasValue
            ? Weight - BodyFatWeight : null;

        public DateTime LocalTimestamp => Timestamp;
    }
}
