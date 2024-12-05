/// <sumary>
/// Stores the calculated BMI, TDEE, and daily caloric intake based on the goal.
/// </sumary>

namespace MacroNutrientCalc.Models
{
    public class CalculationResults
    {
        public double BMI { get; set; }         // Body Mass Index (BMI)
        public double TDEE { get; set; }        // Total Daily Energy Expenditure (TDEE)
        public double Calories { get; set; }    // Adjusted caloric intake based on weight management goal
    }
}
