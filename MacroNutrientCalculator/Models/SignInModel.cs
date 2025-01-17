using System.ComponentModel.DataAnnotations;

namespace MacroNutrientCalculator.Models
{
    public class SignInModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}
