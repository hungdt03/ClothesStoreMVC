using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebBanQuanAo.Data;

namespace WebBanQuanAo.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly StoreDbContext storeDbContext;

        public NotificationController(StoreDbContext storeDbContext)
        {
            this.storeDbContext = storeDbContext;
        }

        public async Task<IActionResult> FindAllByUserId()
        {
            var userId = User?.FindFirst(ClaimTypes.Sid)?.Value ?? "";

            List<Models.Notification> notifications = await storeDbContext.Notifications
                .OrderByDescending(x => x.Id)
                .Take(5)
                .ToListAsync();

            return Ok(notifications);
            
        }
    }
}
