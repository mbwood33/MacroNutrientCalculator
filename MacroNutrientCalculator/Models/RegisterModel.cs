// Model for user registration

using System.ComponentModel.DataAnnotations;

namespace MacroNutrientCalc.Models
{
    // Represents the data required for a user to register
    public class RegisterModel
    {
        public int Id { get; set; } // Id property for storing the user's database ID
        
        [Required(ErrorMessage = "Email is required.")]     // Ensures th Email field is not left empty
        [EmailAddress(ErrorMessage = "Invalid email address.")] // Validates that the Email field contains a valid email format
        public string Email { get; set; } = string.Empty;   // Property for user email; initialized to an empty string

        [Required(ErrorMessage = "Password is required.")]  // Ensures the Password field is not left empty
        public string Password { get; set; } = string.Empty;    // Property for user password; initialized to an empty string

        [Required(ErrorMessage = "Name is required.")]      // Ensures the Name field is not left empty
        public string Name { get; set; } = string.Empty;    // Property for user name; initialized to an empty string

        [Required(ErrorMessage = "Date of Birth is required.")]         // Ensures the Date of Birth field is provided
        public DateTime? DateOfBirth { get; set; } = DateTime.Today;    // Nullable DateTime for date of birth; defaults to today's date
       
        [Required(ErrorMessage = "Height (in) is required.")]   // Ensures that the Height field is not left empty
        [Range(36, 96, ErrorMessage = "Height must be between 36 and 96 inches.")]  // Validates that the Height is between 36 and 96 inches
        public decimal? Height { get; set; }    // Nullable decimal for height input
    }
}
