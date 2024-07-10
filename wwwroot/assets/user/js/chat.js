const btnOpenChat = document.getElementById('site-chat')
const chatModalRightSide = document.getElementById('chat-right-side')
const btnCloseModalChat = document.getElementById('btn-close-chat-side')

btnOpenChat.addEventListener('click', () => {
    chatModalRightSide.classList.toggle('open-chat')
})

btnCloseModalChat.addEventListener('click', () => {
    if (chatModalRightSide.classList.contains('open-chat')) {
        chatModalRightSide.classList.remove('open-chat')
    }
})