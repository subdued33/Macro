namespace CalorieMacroCalculator.Models;

public class MacroProfile
{
    public double ProteinPerKg { get; set; }

    public double FatPerKg { get; set; }

    public double CalorieMultiplier { get; set; } = 1.0;
}