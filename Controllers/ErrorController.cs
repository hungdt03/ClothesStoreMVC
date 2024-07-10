using Microsoft.AspNetCore.Mvc;

namespace WebBanQuanAo.Controllers
{
    public class ErrorController : Controller
    {
       
        public IActionResult HandleErrorCode(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return View("NotFound");
                default:
                    return View("InternalServerError");
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
