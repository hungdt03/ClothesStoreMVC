using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Constants;
using WebBanQuanAo.Data;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly StoreDbContext storeDbContext;

        public PaymentController(StoreDbContext storeDbContext)
        {
            this.storeDbContext = storeDbContext;
        }

        
    }
}
