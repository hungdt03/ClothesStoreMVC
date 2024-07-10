using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Data;
using WebBanQuanAo.Extensions;
using WebBanQuanAo.Models;
using WebBanQuanAo.Payload.Chat;

namespace WebBanQuanAo.Components
{
    public class ChatViewComponent : ViewComponent
    {
        private readonly StoreDbContext storeDbContext;
        private readonly UserManager<User> userManager; 

        public ChatViewComponent(StoreDbContext storeDbContext, UserManager<User> userManager)
        {
            this.storeDbContext = storeDbContext;
            this.userManager = userManager;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            User recipientUser = await userManager.FindByIdAsync("86d88499-5e8b-4865-82b2-e7d87ac64092");

            if(recipientUser == null)
            {
                throw new Exception("Haizzzz");
            }
            var senderUserId = HttpContext.User.GetUserId();

            List<Message> messages = await storeDbContext.Messages
                .Include(msg => msg.Sender)
                .Include(msg => msg.Recipient)
                .Where(msg =>
                    msg.RecipientId.Equals(recipientUser.Id) && msg.SenderId.Equals(senderUserId)
                   || msg.RecipientId.Equals(senderUserId) && msg.SenderId.Equals(recipientUser.Id)
                 )
                .ToListAsync();

            var viewModel = new ChatViewModel()
            {
                RecipientUser = recipientUser,
                CurrentUserId = senderUserId,
                Messages = messages
            };

            return View(viewModel);
        }

    }
}