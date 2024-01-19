"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/Chat").build();
// Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g,
        "&gt;");

    var chatContainer = document.getElementById("messagesList");
    var chatBubble = document.createElement("div");
    chatBubble.classList.add("alert", "alert-info", "m-3");

    var sender = document.createElement("strong");
    sender.textContent = user + ": ";
    chatBubble.appendChild(sender);

    var content = document.createElement("span");
    content.textContent = msg;
    chatBubble.appendChild(content);

    chatContainer.appendChild(chatBubble);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", "", message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
