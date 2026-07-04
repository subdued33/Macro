const trainingStyles = {

    GeneralFitness: [
        "General Fitness",
        "Weight Loss",
        "Muscle Gain"
    ],

    StrengthSports: [
        "Bodybuilding",
        "Powerlifting",
        "Olympic Weightlifting",
        "Strongman"
    ],

    EnduranceSports: [
        "Running",
        "Marathon",
        "Cycling",
        "Swimming",
        "Triathlon"
    ],

    CombatSports: [
        "Boxing",
        "Kickboxing",
        "MMA",
        "Brazilian Jiu-Jitsu",
        "Wrestling"
    ],

    TeamSports: [
        "Football",
        "Basketball",
        "Cricket",
        "Volleyball",
        "Hockey"
    ],

    CrossFit: [
        "CrossFit",
        "HYROX",
        "Functional Fitness"
    ]

};

function updateTrainingStyles() {

    const sport = document.getElementById("sportCategory").value;

    const trainingSelect = document.getElementById("trainingStyle");

    trainingSelect.innerHTML = "";

    trainingStyles[sport].forEach(style => {

        const option = document.createElement("option");

        option.value = style.replace(/\s+/g, "");

        option.textContent = style;

        trainingSelect.appendChild(option);

    });

}



let macroChart = null;

document.addEventListener("DOMContentLoaded", () => {
    

     updateTrainingStyles();

    document
        .getElementById("sportCategory")
        .addEventListener("change", updateTrainingStyles);

    initializeChart();

    const button = document.getElementById("calculateBtn");

    if (button) {
        button.addEventListener("click", calculateMacros);
    }

});

function initializeChart() {
    

    if (typeof Chart === "undefined") {
        console.warn("Chart.js failed to load. Chart disabled.");
        return;
    }

    const canvas = document.getElementById("macroChart");

    if (!canvas) return;

    macroChart = new Chart(canvas, {

        type: "doughnut",

        data: {

            labels: ["Protein", "Carbohydrates", "Fat"],

            datasets: [{

                data: [0, 0, 0],

                backgroundColor: [

                    "#ef4444",
                    "#f59e0b",
                    "#22c55e"

                ],

                borderWidth: 0

            }]

        },

        options: {

            responsive: true,

            maintainAspectRatio: true,

            cutout: "70%",

            plugins: {

                legend: {

                    position: "bottom",

                    labels: {

                        color: "#ffffff",

                        padding: 20,

                        font: {

                            size: 14

                        }

                    }

                }

            }

        }

    });

}

async function calculateMacros() {

    const button = document.getElementById("calculateBtn");
    const buttonText = document.getElementById("buttonText");
    const spinner = document.getElementById("buttonSpinner");

    hideValidation();

    const weight = parseFloat(document.getElementById("weight").value);
    const height = parseFloat(document.getElementById("height").value);
    const age = parseInt(document.getElementById("age").value);

    if (isNaN(weight) || weight <= 0) {

        showValidation("Please enter a valid weight.");

        return;

    }

    if (isNaN(height) || height <= 0) {

        showValidation("Please enter a valid height.");

        return;

    }

    if (isNaN(age) || age < 15 || age > 100) {

        showValidation("Age must be between 15 and 100.");

        return;

    }

    button.disabled = true;

    buttonText.innerText = "Calculating...";

    spinner.classList.remove("d-none");

    const input = {

        weight: weight,

        weightUnit: document.getElementById("weightUnit").value,

        height: height,

        age: age,

        gender: document.getElementById("gender").value,

        activityLevel: document.getElementById("activityLevel").value,

        goal: document.getElementById("goal").value,

        proteinPreference: document.getElementById("proteinPreference").value,

        sportCategory: document.getElementById("sportCategory").value,

        trainingStyle: document.getElementById("trainingStyle").value

    };

    try {

        const response = await fetch("/Calculator/Calculate", {

            method: "POST",

            headers: {

                "Content-Type": "application/json"

            },

            body: JSON.stringify(input)

        });

        if (!response.ok) {

            throw new Error("Unable to calculate macros.");

        }

        const result = await response.json();

       animateNumber("targetCalories", result.targetCalories, " kcal");
animateNumber("maintenanceCalories", result.maintenanceCalories, " kcal");
animateNumber("bmr", result.bmr, " kcal");
animateNumber("protein", result.protein, " g");
animateNumber("carbs", result.carbs, " g");
animateNumber("fat", result.fat, " g");

        if (macroChart &&
            result.proteinCalories !== undefined &&
            result.carbCalories !== undefined &&
            result.fatCalories !== undefined) {

            macroChart.data.datasets[0].data = [

                result.proteinCalories,

                result.carbCalories,

                result.fatCalories

            ];

            macroChart.update('active');

            const total =
                result.proteinCalories +
                result.carbCalories +
                result.fatCalories;

            document.getElementById("proteinBar").style.width =
                ((result.proteinCalories / total) * 100) + "%";

            document.getElementById("carbBar").style.width =
                ((result.carbCalories / total) * 100) + "%";

            document.getElementById("fatBar").style.width =
                ((result.fatCalories / total) * 100) + "%";

        }

    }
    catch (error) {

        showValidation(error.message);

    }
    finally {

        button.disabled = false;

        buttonText.innerText = "Calculate Macros";

        spinner.classList.add("d-none");

    }

}

function animateNumber(id, endValue, suffix = "") {

    const element = document.getElementById(id);

    if (!element) return;

    const startValue = parseInt(element.innerText.replace(/[^\d]/g, "")) || 0;

    const duration = 1000; // 1 second
    const frameRate = 60;
    const totalFrames = Math.round(duration / (1000 / frameRate));

    let frame = 0;

    const counter = setInterval(() => {

        frame++;

        const progress = frame / totalFrames;

        // Ease-out animation
        const easedProgress = 1 - Math.pow(1 - progress, 3);

        const currentValue = Math.round(
            startValue + (endValue - startValue) * easedProgress
        );

        element.innerText = currentValue + suffix;

        if (frame >= totalFrames) {

            clearInterval(counter);

            element.innerText = endValue + suffix;

        }

    }, 1000 / frameRate);
}

function showValidation(message) {

    const validation = document.getElementById("validationMessage");

    if (!validation) return;

    validation.innerText = message;

    validation.classList.remove("d-none");

}

function hideValidation() {

    const validation = document.getElementById("validationMessage");

    if (!validation) return;

    validation.classList.add("d-none");

    validation.innerText = "";

}