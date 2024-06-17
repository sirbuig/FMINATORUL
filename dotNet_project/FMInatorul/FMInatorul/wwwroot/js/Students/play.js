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
        joinMultiplayerRoom();
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

    // SignalR connection setup
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/gameHub")
        .build();

    connection.start().then(function () {
        console.log("SignalR Connected.");
    }).catch(function (err) {
        return console.error(err.toString());
    });

    // Function to join multiplayer room
    function joinMultiplayerRoom() {
        const roomName = "multiplayerRoom"; 
        connection.invoke("JoinRoom", roomName).catch(function (err) {
            return console.error(err.toString());
        });
        console.log("Joined room: " + roomName);
    }

    // Handle PlayerJoined and PlayerLeft events
    connection.on("PlayerJoined", function (playerId) {
        console.log(playerId + " joined the room.");
    });

    connection.on("PlayerLeft", function (playerId) {
        console.log(playerId + " left the room.");
    });
});
