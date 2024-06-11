"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var div = document.createElement("div");
    div.classList.add("chat-message");

    var userNameDiv = document.createElement("div");
    userNameDiv.classList.add("user-name");
    userNameDiv.textContent = user;

    var messageDiv = document.createElement("div");
    messageDiv.classList.add("message");
    messageDiv.textContent = message;

    div.appendChild(userNameDiv);
    div.appendChild(messageDiv);

    // Check if the message is from the current user
    if (user === document.getElementById("userInput").value) {
        div.classList.add("user");
    } else {
        div.classList.add("other");
    }

    document.getElementById("messagesList").appendChild(div);
    document.getElementById("messagesList").scrollTop = document.getElementById("messagesList").scrollHeight;
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
