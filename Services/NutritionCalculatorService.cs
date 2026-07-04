using CalorieMacroCalculator.Models;

namespace CalorieMacroCalculator.Services;

public class NutritionCalculatorService
{
    public CalculatorResult Calculate(CalculatorInput input)
    {
        // -----------------------------
        // Weight Conversion
        // -----------------------------
        double weightKg = input.Weight;

        if (input.WeightUnit == "lbs")
            weightKg *= 0.453592;

        // -----------------------------
        // BMR (Mifflin-St Jeor)
        // -----------------------------
        double bmr = input.Gender == "Male"
            ? (10 * weightKg) + (6.25 * input.Height) - (5 * input.Age) + 5
            : (10 * weightKg) + (6.25 * input.Height) - (5 * input.Age) - 161;

        // -----------------------------
        // Activity Multiplier
        // -----------------------------
        double activityMultiplier = input.ActivityLevel switch
        {
            "Sedentary" => 1.20,
            "LightlyActive" => 1.375,
            "ModeratelyActive" => 1.55,
            "VeryActive" => 1.725,
            "ExtraActive" => 1.90,
            _ => 1.55
        };

        double maintenanceCalories = bmr * activityMultiplier;

        // -----------------------------
        // Goal Calories
        // -----------------------------
        double targetCalories = input.Goal switch
        {
            "Cut" => maintenanceCalories * 0.82,      // ~18% deficit
            "Bulk" => maintenanceCalories * 1.10,     // 10% surplus
            "Recomp" => maintenanceCalories * 0.98,   // slight deficit
            _ => maintenanceCalories
        };

        // Never recommend dangerously low calories
        targetCalories = Math.Max(targetCalories, bmr * 1.20);

        // -----------------------------
        // Protein
        // -----------------------------
        double proteinPerKg = input.Goal switch
        {
            "Cut" => 2.2,
            "Bulk" => 1.8,
            "Recomp" => 2.2,
            _ => 1.6
        };

        if (input.ProteinPreference == "High")
            proteinPerKg += 0.2;

        double protein = weightKg * proteinPerKg;

        // -----------------------------
        // Fat
        // -----------------------------
        double fatPerKg = input.Goal switch
        {
            "Cut" => 0.7,
            "Bulk" => 0.9,
            "Recomp" => 0.8,
            _ => 0.8
        };

        double fat = weightKg * fatPerKg;

        // Never recommend extremely low fat intake
        fat = Math.Max(fat, 40);

        // -----------------------------
        // Calories From Protein & Fat
        // -----------------------------
        double proteinCalories = protein * 4;
        double fatCalories = fat * 9;

        // -----------------------------
        // Ensure Calories Remain For Carbs
        // -----------------------------
        double remainingCalories = targetCalories - proteinCalories - fatCalories;

        // If calories become negative,
        // reduce protein first,
        // then fat (never below minimum)
        if (remainingCalories < 0)
        {
            double deficit = Math.Abs(remainingCalories);

            double removableProtein = Math.Max(0, protein - (1.6 * weightKg));
            double removableProteinCalories = removableProtein * 4;

            if (removableProteinCalories >= deficit)
            {
                protein -= deficit / 4;
            }
            else
            {
                protein -= removableProtein;

                deficit -= removableProteinCalories;

                double removableFat = Math.Max(0, fat - 40);

                fat -= Math.Min(removableFat, deficit / 9);
            }

            proteinCalories = protein * 4;
            fatCalories = fat * 9;

            remainingCalories = targetCalories - proteinCalories - fatCalories;
        }

        // Final safeguard
        remainingCalories = Math.Max(remainingCalories, 0);

        double carbs = remainingCalories / 4;

        return new CalculatorResult
        {
            Bmr = Math.Round(bmr),

            MaintenanceCalories = Math.Round(maintenanceCalories),

            TargetCalories = Math.Round(targetCalories),

            Protein = Math.Round(protein),

            Carbs = Math.Round(carbs),

            Fat = Math.Round(fat),

            ProteinCalories = Math.Round(proteinCalories),

            FatCalories = Math.Round(fatCalories),

            CarbCalories = Math.Round(remainingCalories)
        };
    }
}