using System.Security.Claims;

namespace WebBanQuanAo.Middleware
{
    public class AdminAreaAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AdminAreaAuthorizationMiddleware> _logger;

        public AdminAreaAuthorizationMiddleware(RequestDelegate next, ILogger<AdminAreaAuthorizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/Admin"))
            {
                var roles = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
                
                foreach (var role in roles)
                {
                    _logger.LogWarning(role);
                }

                if (!context.User.IsInRole("Admin"))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }

            await _next(context);
        }
    }
}
