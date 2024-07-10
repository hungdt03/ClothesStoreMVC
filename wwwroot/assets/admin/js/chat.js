

document.addEventListener('DOMContentLoaded', (event) => {
    const chatArea = document.querySelector(".chat-messages");
    console.log("DOM NÈ")
    if (chatArea) {
        console.log("Đây nà")
        chatArea.scrollTo({
            top: chatArea.scrollHeight,
            behavior: 'smooth'
        });
    }

    const chatConnection = new signalR.HubConnectionBuilder()
        .withUrl("/messageHub")
        .build();

    const targetUsername = document.getElementById('target-username')
    const messageContent = document.getElementById("input-message");

    if (targetUsername) {
        console.log(targetUsername)
    }

    document.getElementById("btn-send-message").addEventListener("click", function () {

        const username = targetUsername.getAttribute('data-username')

        if (username && messageContent && messageContent.value) {
            chatConnection.invoke("SendMessage", { recipientUsername: username, content: messageContent.value })
                .catch(err => console.error(err.toString()));
        }

    });

    function getHourAndMinute(dateString) {
        const date = new Date(dateString);

        const hours = date.getHours().toString().padStart(2, '0');
        const minutes = date.getMinutes().toString().padStart(2, '0');

        return `${hours}:${minutes}`;
    }

    chatConnection.on("NewMessage", function (newMessage) {
        console.log(newMessage)
        renderUIChat(newMessage)
    });

    chatConnection.start().catch(function (err) {
        return console.error(err.toString());
    });

    function renderUIChat(newMessage) {
        const messageWrapper = document.getElementById('message-wrapper');
        const username = targetUsername.getAttribute('data-username')

        var html = "";
        console.log(newMessage)
        console.log(newMessage.recipientUser.userName)
        console.log(username)

        if (newMessage.recipientUser.userName != username) {
            html = `
            <div class="message">
                 <div class="message-content">
                       <span>${newMessage.content}</span>
                       <span class="message-time">${getHourAndMinute(newMessage.sentAt)}</span>
                 </div>
            </div>
        `
        } else {
            html = `
             <div class="message own-message">
                        <div class="message-content">
                            <span>${newMessage.content}</span>
                            <span class="message-time">${getHourAndMinute(newMessage.sentAt)}</span>
                        </div>
            </div>
        `
        }

        messageWrapper.innerHTML += html;
        messageContent.value = "";
        scrollToBottom();
    }

    function scrollToBottom() {
        const chatArea = document.querySelector(".chat-messages");

        chatArea.scrollTo({
            top: chatArea.scrollHeight,
            behavior: 'smooth'
        });
    }
  
});
