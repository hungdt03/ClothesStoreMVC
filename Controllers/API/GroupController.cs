using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebBanQuanAo.Data;
using WebBanQuanAo.Extensions;
using WebBanQuanAo.Services.Interfaces;

namespace WebBanQuanAo.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService groupService;

        public GroupController(IGroupService groupService)
        {
            this.groupService = groupService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FindAllGroupByCurrentUser()
        {
            var currentUsername = HttpContext.User.GetUsername();
            var allGroups = await groupService.FindAllByUsername(currentUsername);
            return Ok(allGroups);
        }
    }
}
