"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/groupchat")
    .build();


// Get groupId from ViewBag
const groupId = document.getElementById("group-chat-container").getAttribute("data-group-id");

// Start connection and join group
connection.start().then(function () {
    connection.invoke("JoinGroup", groupId);
}).catch(function (err) {
    return console.error(err.toString());
});

// Handle receiving message
connection.on("ReceiveGroupMessage", function (user, message, sentAt) {
    const chatContainer = document.getElementById("group-chat-container");
    const msgDiv = document.createElement("div");
    msgDiv.classList.add("message");

    const timestamp = new Date(sentAt).toLocaleTimeString();
    msgDiv.innerHTML = `<strong>${user}</strong>: ${message} <span class="text-muted" style="font-size: 0.8em;">(${timestamp})</span>`;
    chatContainer.appendChild(msgDiv);
    chatContainer.scrollTop = chatContainer.scrollHeight;
});

// Send message
document.getElementById("send-group-msg-btn").addEventListener("click", function (event) {
    event.preventDefault();

    const msgInput = document.getElementById("group-msg-input");
    const message = msgInput.value;

    if (message.trim() !== "") {
        connection.invoke("SendGroupMessage", groupId, message).catch(function (err) {
            return console.error(err.toString());
        });
        msgInput.value = "";
    }
});
