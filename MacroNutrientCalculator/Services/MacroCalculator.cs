using MacroNutrientCalc.Models;

namespace MacroNutrientCalc.Services
{
    /// <summary>
    /// Contains methods to perform calculations for macronutrient breakdown, BMI, BMR, and TDEE.
    /// </summary>
    public static class MacroCalculator
    {
        /// <summary>
        /// Calculates BMI, TDEE, and adjusted daily caloric intake based on user's stats and goals.
        /// </summary>
        /// <param name="stats">The user's physical stats such as weight, height, age, and sex.</param>
        /// <param name="goal">The user's goal (e.g., "Lose Weight", "Gain Weight") and rate of change.</param>
        /// <returns>A CalculationResults object containing BMI, TDEE, and adjusted calories.</returns>
        public static CalculationResults Calculate(UserStats stats, GoalSettings goal)
        {
            double weight = stats.Weight ?? 0;  // Use 0 as the default value for null
            double height = stats.Height ?? 0;
            int age = stats.Age ?? 0;

            double bmi = CalculateBMI(weight, height);                      // Calculate Body Mass Index (BMI)
            double bmr = CalculateBMR(weight, height, age, stats.Sex);      // Calculate Basal Metabolic Rate (BMR) using the Harris-Benedict equation
            double tdee = bmr * GetActivityMultiplier(stats.ActivityLevel); // Total Daily Energy Expenditure (TDEE) = BMR * activity multiplier

            double calorieAdjustment = goal.Type == "Lose Weight"
                ? -3500 * double.Parse(goal.Rate ?? "0") / 7
                : goal.Type == "Gain Weight"
                    ? 3500 * double.Parse(goal.Rate ?? "0") / 7
                    : 0;

            return new CalculationResults
            {
                BMI = bmi,
                TDEE = tdee,
                Calories = tdee + calorieAdjustment
            };

            /*
            
            // Calculate Body Mass Index (BMI)
            double bmi = CalculateBMI(stats.Weight, stats.Height);

            // Calculate Basal Metabolic Rate (BMR) using the Harris-Benedict equation
            double bmr = CalculateBMR(stats.Weight, stats.Height, stats.Age, stats.Sex);

            // Total Daily Energy Expenditure (TDEE) = BMR * activity multiplier
            double tdee = bmr * GetActivityMultiplier(stats.ActivityLevel);

            // Adjust caloric intake based on the user's goal
            // Lose/Gain 1 lb per week = ±3500 calories over a week
            // Daily adjustment = (±3500 calories * rate in lbs) / 7 days
            double calorieAdjustment = goal.Type == "Lose Weight"
                ? -3500 * double.Parse(goal.Rate) / 7
                : goal.Type == "Gain Weight"
                    ? 3500 * double.Parse(goal.Rate) / 7
                    : 0;

            // Return calculated results
            return new CalculationResults
            {
                BMI = bmi,
                TDEE = tdee,
                Calories = tdee + calorieAdjustment
            };

            */
        }

        /// <summary>
        /// Calculates BMI (Body Mass Index) using the formula:
        /// BMI = (Weight in lbs) / (Height in inches)^2 * 703
        /// </summary>
        /// <param name="weight">The user's weight in pounds (lbs).</param>
        /// <param name="height">The user's height in inches.</param>
        /// <returns>The calculated BMI value.</returns>
        private static double CalculateBMI(double weight, double height)
            => (weight / (height * height)) * 703;

        /// <summary>
        /// Calculates BMR (Basal Metabolic Rate) using the Harris-Benedict equation.
        /// The equation differs based on sex:
        /// - Male: BMR = 66 + (6.23 * weight) + (12.7 * height) - (6.8 * age)
        /// - Female: BMR = 655 + (4.35 * weight) + (4.7 * height) - (4.7 * age)
        /// </summary>
        /// <param name="weight">The user's weight in pounds (lbs).</param>
        /// <param name="height">The user's height in inches.</param>
        /// <param name="age">The user's age in years.</param>
        /// <param name="sex">The user's biological sex ("Male" or "Female").</param>
        /// <returns>The calculated BMR value in calories/day.</returns>
        private static double CalculateBMR(double weight, double height, int age, string? sex)
        {
            if (sex == "Male")
            {
                return 66 + (6.23 * weight) + (12.7 * height) - (6.8 * age);
            }
            return 655 + (4.35 * weight) + (4.7 * height) - (4.7 * age);
        }

        /// <summary>
        /// Determines the activity multiplier based on the user's reported activity level.
        /// Activity levels and their multipliers:
        /// - Sedentary: 1.2
        /// - Lightly Active: 1.375
        /// - Moderately Active: 1.55
        /// - Very Active: 1.725
        /// - Extremely Active: 1.9
        /// </summary>
        /// <param name="level">The user's activity level.</param>
        /// <returns>The activity multiplier for TDEE calculation.</returns>
        private static double GetActivityMultiplier(string? level)
        {
            return level switch
            {
                "Sedentary" => 1.2,
                "Lightly Active" => 1.375,
                "Moderately Active" => 1.55,
                "Very Active" => 1.725,
                "Extremely Active" => 1.9,
                _ => 1.2 // Default to Sedentary if no level is specified
            };
        }

        /// <summary>
        /// Calculates macronutrient distribution (grams of carbs, protein, and fat) based on total calories and a specified macro ratio.
        /// Macronutrient calorie equivalents:
        /// - 1 gram of carbohydrates = 4 calories
        /// - 1 gram of protein = 4 calories
        /// - 1 gram of fat = 9 calories
        /// </summary>
        /// <param name="calories">The total daily caloric intake.</param>
        /// <param name="macroRatio">The desired macronutrient ratio (e.g., "Keto", "Zone", "Standard").</param>
        /// <returns>A MacroResults object containing grams of carbs, protein, and fat.</returns>
        public static MacroResults CalculateMacronutrients(double calories, string macroRatio)
        {
            double carbRatio, proteinRatio, fatRatio;

            // Define macro ratios for different plans
            switch (macroRatio)
            {
                case "Keto":
                    carbRatio = 0.05; proteinRatio = 0.20; fatRatio = 0.75;
                    break;
                case "Zone":
                    carbRatio = 0.40; proteinRatio = 0.30; fatRatio = 0.30;
                    break;
                case "Standard":
                default:
                    carbRatio = 0.50; proteinRatio = 0.20; fatRatio = 0.30;
                    break;
            }

            // Calculate macronutrients in grams
            return new MacroResults
            {
                Carbs = (calories * carbRatio) / 4,   // Convert carb calories to grams
                Protein = (calories * proteinRatio) / 4, // Convert protein calories to grams
                Fat = (calories * fatRatio) / 9       // Convert fat calories to grams
            };
        }
    }
}
