using CalorieMacroCalculator.Models;
using CalorieMacroCalculator.Services;
using Microsoft.AspNetCore.Mvc;

namespace CalorieMacroCalculator.Controllers;

public class CalculatorController : Controller
{
    private readonly NutritionCalculatorService _calculatorService;

    public CalculatorController(NutritionCalculatorService calculatorService)
    {
        _calculatorService = calculatorService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Calculate([FromBody] CalculatorInput input)
    {
        var result = _calculatorService.Calculate(input);

        return Json(result);
    }
}