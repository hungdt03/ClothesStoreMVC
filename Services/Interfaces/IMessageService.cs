using WebBanQuanAo.Models;

namespace WebBanQuanAo.Services.Interfaces
{
    public interface IMessageService
    {
        Task<Message> AddMessage(Message message);
        Task<List<Message>> GetMessagesBySenderIdAndRecipientId(string senderId, string recipientId);
    }
}
