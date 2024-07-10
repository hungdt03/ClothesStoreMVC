using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebBanQuanAo.Models;
using WebBanQuanAo.Services.Interfaces;

namespace WebBanQuanAo.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserService userService;

        public UserController(UserManager<User> userManager, IUserService userService)
        {
            _userManager = userManager;
            this.userService = userService;
        }

        public async Task<IActionResult> GetAll()
        {
            List<User> users = await userService.FindAll();
            return Ok(users);
        }
    }
}
