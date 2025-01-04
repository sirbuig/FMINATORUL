document.addEventListener('DOMContentLoaded', function () {
    const singleplayerBtn = document.getElementById('singleplayerBtn');
    const multiplayerBtn = document.getElementById('multiplayerBtn');
    const materialsQuestion = document.getElementById('materialsQuestion');
    const myMaterialsBtn = document.getElementById('myMaterials');
    const yourMaterialsBtn = document.getElementById('yourMaterials');
    const materialsForm = document.getElementById('materialsForm');
    const multiplayerChoice = document.getElementById('multiplayerChoice');
    const joinRoomBtn = document.getElementById('joinRoom');
    const hostRoomBtn = document.getElementById('hostRoom');
    const joinGameForm = document.getElementById('joinGameForm');

    function toggleGameMode(selectedMode) {
        if (selectedMode === 'singleplayer') {
            materialsQuestion.style.display = 'block';
            multiplayerChoice.style.display = 'none';
            singleplayerBtn.classList.add('btn-active');
            multiplayerBtn.classList.remove('btn-active');
            joinGameForm.style.display = 'none';
        } else if (selectedMode === 'multiplayer') {
            materialsQuestion.style.display = 'none';
            multiplayerChoice.style.display = 'block';
            multiplayerBtn.classList.add('btn-active');
            singleplayerBtn.classList.remove('btn-active');
            materialsForm.style.display = 'none';
        }
    }

    singleplayerBtn.addEventListener('click', function () {
        toggleGameMode('singleplayer');
    });

    multiplayerBtn.addEventListener('click', function () {
        console.log('Multiplayer button clicked');
        toggleGameMode('multiplayer');
    });

    joinRoomBtn.addEventListener('click', function () {
        this.classList.add('btn-active');
        hostRoomBtn.classList.remove('btn-active');
        joinGameForm.style.display = 'block';
    });

    hostRoomBtn.addEventListener('click', function () {
        this.classList.add('btn-active');
        joinRoomBtn.classList.remove('btn-active');
        joinGameForm.style.display = 'none';
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


    // for the room
    hostRoomBtn.addEventListener('click', async function () {
        // CreateRoom endpoint
        const response = await fetch('/Rooms/CreateRoom', { method: 'POST' });
        const data = await response.json();

        // did we get the code?
        if (data.code) {
            window.location.href = `/Rooms/Lobby?code=${data.code}`;
            connection.invoke('JoinRoomGroup', data.code);
        } else {
            // :(
            alert('Could not create room');
        }
    });

    joinGameForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const code = document.getElementById('gameCode').value;

        const response = await fetch('/Rooms/JoinRoom', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ code })
        });

        const data = await response.json();
        if (data.success) {
            //alert(data.message);
            window.location.href = `/Rooms/Lobby?code=${code}`;
            connection.invoke("JoinRoomGroup", code);
        } else {
            alert(data.message);
        }
    });

});
