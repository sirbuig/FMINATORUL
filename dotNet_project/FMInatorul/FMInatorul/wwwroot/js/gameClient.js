"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/gameHub")
    .build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;
connection.on("PlayerJoined", function (playerId) {
    console.log(playerId + " joined the room.");
});

connection.on("PlayerLeft", function (playerId) {
    console.log(playerId + " left the room.");
});

connection.start().then(function () {
    console.log("SignalR Connected.");
}).catch(function (err) {
    return console.error(err.toString());
});

function joinRoom(roomName) {
    connection.invoke("JoinRoom", roomName).catch(function (err) {
        return console.error(err.toString());
    });
}

function leaveRoom(roomName) {
    connection.invoke("LeaveRoom", roomName).catch(function (err) {
        return console.error(err.toString());
    });
}
