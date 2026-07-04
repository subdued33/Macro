namespace CalorieMacroCalculator.Models;

public class CalculatorInput
{
    public double Weight { get; set; }

    public string WeightUnit { get; set; } = "kg";

    public double Height { get; set; }

    public int Age { get; set; }

    public string Gender { get; set; } = "Male";

    public string ActivityLevel { get; set; } = "ModeratelyActive";

    public string Goal { get; set; } = "Maintain";

    public string ProteinPreference { get; set; } = "Standard";

    public string SportCategory { get; set; } = "GeneralFitness";

    public string TrainingStyle { get; set; } = "GeneralFitness";
}