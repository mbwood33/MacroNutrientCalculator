// Model for user registration

using System;
using System.ComponentModel.DataAnnotations;

namespace MacroNutrientCalc.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of Birth is required.")]
        public DateTime DateOfBirth { get; set; } = DateTime.Today;
       
        [Required(ErrorMessage = "Height (in) is required.")]
        public decimal Height { get; set; }
    }
}
