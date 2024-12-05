/// <sumary>
/// Represents the distribution of macronutrients (carbs, protein, fat) in grams.
/// </sumary>

namespace MacroNutrientCalc.Models
{
    public class MacroResults
    {
        public double Carbs { get; set; }       // Carbohydrates in grams
        public double Protein { get; set; }     // Protein in grams
        public double Fat {  get; set; }        // Fat in grams
    }
}
