using CalorieMacroCalculator.Models;

namespace CalorieMacroCalculator.Services;

public class NutritionCalculatorService
{
    public CalculatorResult Calculate(CalculatorInput input)
    {
        double weightKg = input.Weight;

        if (input.WeightUnit == "lbs")
        {
            weightKg = input.Weight * 0.453592;
        }

        // BMR (Mifflin-St Jeor)
        double bmr;

        if (input.Gender == "Male")
        {
            bmr = (10 * weightKg)
                + (6.25 * input.Height)
                - (5 * input.Age)
                + 5;
        }
        else
        {
            bmr = (10 * weightKg)
                + (6.25 * input.Height)
                - (5 * input.Age)
                - 161;
        }

        // Activity multiplier
        double activityMultiplier = input.ActivityLevel switch
        {
            "Sedentary" => 1.2,
            "LightlyActive" => 1.375,
            "ModeratelyActive" => 1.55,
            "VeryActive" => 1.725,
            "ExtraActive" => 1.9,
            _ => 1.55
        };

        double maintenance = bmr * activityMultiplier;

        double targetCalories = input.Goal switch
        {
            "Cut" => maintenance * 0.80,
            "Bulk" => maintenance + 300,
            "Recomp" => maintenance,
            _ => maintenance
        };

        double proteinPerKg = input.Goal switch
        {
            "Cut" => 2.2,
            "Bulk" => 1.8,
            "Recomp" => 2.2,
            _ => 1.6
        };

        if (input.ProteinPreference == "High")
        {
            proteinPerKg += 0.2;
        }

        double protein = proteinPerKg * weightKg;

        double fat = input.Goal switch
        {
            "Cut" => 0.7 * weightKg,
            "Bulk" => 0.9 * weightKg,
            _ => 0.8 * weightKg
        };

        double proteinCalories = protein * 4;
        double fatCalories = fat * 9;

        double carbCalories = targetCalories - proteinCalories - fatCalories;

        double carbs = carbCalories / 4;

        return new CalculatorResult
        {
            Bmr = Math.Round(bmr),

            MaintenanceCalories = Math.Round(maintenance),

            TargetCalories = Math.Round(targetCalories),

            Protein = Math.Round(protein),

            Carbs = Math.Round(carbs),

            Fat = Math.Round(fat),

            ProteinCalories = Math.Round(proteinCalories),

            FatCalories = Math.Round(fatCalories),

            CarbCalories = Math.Round(carbCalories)
        };
    }
}