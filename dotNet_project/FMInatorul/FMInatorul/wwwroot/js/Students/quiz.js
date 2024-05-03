let currentQuestion = 0;
const questionBoxes = document.querySelectorAll(".question-box");
const prevButton = document.getElementById("prev");
const nextButton = document.getElementById("next");
const submitButton = document.querySelector('input[type="submit"]');

if (questionBoxes.length > 0) {
    questionBoxes[0].style.display = 'block';
    checkButtons();
}

function showQuestion(index) {
    questionBoxes.forEach(box => box.style.display = "none");
    questionBoxes[index].style.display = "block";
}

prevButton.addEventListener('click', () => {
    if (currentQuestion > 0) {
        currentQuestion--;
        showQuestion(currentQuestion);
        checkButtons();
    }
});

nextButton.addEventListener('click', () => {
    if (!validAnswer()) {
        alert("Trebuie sa selectezi un raspuns.");
        return;
    }

    if (currentQuestion < questionBoxes.length - 1) {
        currentQuestion++;
        showQuestion(currentQuestion);
        checkButtons();
    }
});

function validAnswer() {
    const currentBox = questionBoxes[currentQuestion];
    const radios = currentBox.querySelectorAll('input[type="radio"]');
    return Array.from(radios).some(radio => radio.checked);
}

function checkButtons() {
    prevButton.disabled = currentQuestion === 0;
    nextButton.disabled = currentQuestion === questionBoxes.length - 1;
    submitButton.style.display = currentQuestion === questionBoxes.length - 1 ? "block" : "none";
}
