document.addEventListener('DOMContentLoaded', function () {
    const singleplayerBtn = document.getElementById('singleplayerBtn');
    const multiplayerBtn = document.getElementById('multiplayerBtn');
    const materialsQuestion = document.getElementById('materialsQuestion');
    const myMaterialsBtn = document.getElementById('myMaterials');
    const yourMaterialsBtn = document.getElementById('yourMaterials');
    const materialsForm = document.getElementById('materialsForm');

    function toggleGameMode(selectedMode) {
        if (selectedMode === 'singleplayer') {
            materialsQuestion.style.display = 'block';
            singleplayerBtn.classList.add('btn-active');
            multiplayerBtn.classList.remove('btn-active');
        } else {
            materialsQuestion.style.display = 'none';
            multiplayerBtn.classList.add('btn-active');
            singleplayerBtn.classList.remove('btn-active');
            myMaterialsBtn.classList.remove('btn-active');
            yourMaterialsBtn.classList.remove('btn-active');
            materialsForm.style.display = 'none';
        }
    }

    singleplayerBtn.addEventListener('click', function () {
        toggleGameMode('singleplayer');
    });

    multiplayerBtn.addEventListener('click', function () {
        toggleGameMode('multiplayer');
    });

    myMaterialsBtn.addEventListener('click', function () {
        this.classList.add('btn-active');
        yourMaterialsBtn.classList.remove('btn-active');
        materialsForm.style.display = 'block';
    });

    yourMaterialsBtn.addEventListener('click', function () {
        this.classList.add('btn-active');
        myMaterialsBtn.classList.remove('btn-active');
        materialsForm.style.display = 'none';
    });
});