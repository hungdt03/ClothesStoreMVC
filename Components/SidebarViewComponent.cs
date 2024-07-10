using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Data;
using WebBanQuanAo.Extensions;
using WebBanQuanAo.Models;
using WebBanQuanAo.Payload.Components;

namespace WebBanQuanAo.Components
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly StoreDbContext storeDbContext;

        public SidebarViewComponent(StoreDbContext storeDbContext)
        {
            this.storeDbContext = storeDbContext;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUsername = HttpContext.User.GetUsername();

            List<User> users = await storeDbContext.Users
                .Where(u => !u.UserName.Equals(currentUsername))
                .ToListAsync();

            SidebarModelView model = new SidebarModelView();
            model.Users = users;

            return View(model);
        }

    }
}