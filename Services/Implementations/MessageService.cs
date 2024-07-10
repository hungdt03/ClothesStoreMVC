using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WebBanQuanAo.Data;
using WebBanQuanAo.Models;
using WebBanQuanAo.Services.Interfaces;

namespace WebBanQuanAo.Services.Implementations
{
    public class MessageService : IMessageService
    {
        private readonly StoreDbContext _dbContext;

        public MessageService(StoreDbContext dbContext) { 
            this._dbContext = dbContext;
        }
        public async Task<Message> AddMessage(Message message)
        {
            EntityEntry<Message> savedMessage = await _dbContext.Messages.AddAsync(message);

            await _dbContext.Entry(savedMessage.Entity)
                .Reference(m => m.Sender)
                .LoadAsync();

            await _dbContext.Entry(savedMessage.Entity)
                            .Reference(m => m.Recipient)
                            .LoadAsync();

            await _dbContext.SaveChangesAsync();

            return savedMessage.Entity;
        }

        public async Task<List<Message>> GetMessagesBySenderIdAndRecipientId(string senderId, string recipientId)
        {
            List<Message> messages = await _dbContext.Messages
                .Where(
                    msg => msg.RecipientId.Equals(recipientId) && msg.SenderId.Equals(senderId)
                ).ToListAsync();

            return messages;
        }
    }
}
