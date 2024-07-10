using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Data;
using WebBanQuanAo.Extensions;
using WebBanQuanAo.Models;
using WebBanQuanAo.Payload.Chat;

namespace WebBanQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ChatController : AdminController
    {
        private readonly UserManager<User> userManager;
        private readonly StoreDbContext storeDbContext;

        public ChatController(UserManager<User> userManager, StoreDbContext storeDbContext)
        {
            this.userManager = userManager;
            this.storeDbContext = storeDbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(string id)
        {
            User recipientUser = await userManager.FindByIdAsync(id);
            var senderUserId = HttpContext.User.GetUserId();

            List<Message> messages = await storeDbContext.Messages
                .Include(msg => msg.Sender)
                .Include(msg => msg.Recipient)
                .Where(msg => 
                    msg.RecipientId.Equals(id) && msg.SenderId.Equals(senderUserId)
                   || msg.RecipientId.Equals(senderUserId) && msg.SenderId.Equals(id)
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
