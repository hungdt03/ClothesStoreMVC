using WebBanQuanAo.Models;

namespace WebBanQuanAo.Payload.Chat
{
    public class ChatViewModel
    {
        public User RecipientUser { get; set; }
        public List<Message> Messages { get; set; }
        public string CurrentUserId { get; set; }
    }
}
