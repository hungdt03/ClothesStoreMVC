using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using WebBanQuanAo.Models;
using WebBanQuanAo.Constants;
using WebBanQuanAo.Extensions;
using WebBanQuanAo.Payload.Sessions;
using WebBanQuanAo.Data;
using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Payload.Auth;

namespace WebBanQuanAo.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly StoreDbContext _storeDbContext;

        public AuthController(SignInManager<User> signInManager, UserManager<User> userManager, ILogger<AuthController> logger, StoreDbContext storeDbContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _storeDbContext = storeDbContext;
        }

        public async Task<IActionResult> Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            List<AuthenticationScheme?> externalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewBag.ExternalLogins = externalLogins;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Oauth2Login(string provider)
        {
            var listprovider = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            var provider_process = listprovider.Find((m) => m.Name == provider);

            _logger.LogInformation(provider_process.ToString());
            if (provider_process == null)
            {
                return NotFound("Dịch vụ không chính xác: " + provider);
            }

            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Oauth2Callback")
            };
            return Challenge(authenticationProperties, provider);

        
        }

        public async Task<IActionResult> Oauth2Callback()
        {
            var result = HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result.Result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            // retrieve claims of the external user
            var externalUser = result.Result.Principal;
            if (externalUser == null)
            {
                throw new Exception("External authentication error");
            }

            // retrieve claims of the external user
            var claims = externalUser.Claims.ToList();

            var userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var userNameClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
            if (userIdClaim == null)
            {
                throw new Exception("Unknown userid");
            }

            var externalUserId = userIdClaim.Value;
            var externalProvider = userIdClaim.Issuer;

           
            // use externalProvider and externalUserId to find your user, or provision a new user
            var user = await _userManager.FindByLoginAsync(externalProvider, externalUserId);

            if (user == null)
            {
                var emailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);          
                var userEmail = emailClaim.Value;

                var existingUser = await _userManager.FindByEmailAsync(userEmail);

                if (existingUser != null)
                {
                  
                    user = existingUser;

                    var addExternalLoginResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(externalProvider, externalUserId, externalProvider));
                    if (!addExternalLoginResult.Succeeded)
                    {
                        throw new Exception($"Cannot add Google login: {addExternalLoginResult.Errors.FirstOrDefault()?.Description}");
                    }
                } else
                {
                    user = new User
                    {
                        UserName = userEmail,
                        Email = userEmail,
                        IsActive = true,
                        FullName = userNameClaim.Value
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        throw new Exception($"Cannot create user: {createResult.Errors.FirstOrDefault()?.Description}");
                    }

                    var addToRoleResult = await _userManager.AddToRoleAsync(user, RoleConstant.ROLE_CUSTOMER);
                    if (!addToRoleResult.Succeeded)
                    {
                        throw new Exception($"Failed to add role to user: {addToRoleResult.Errors.FirstOrDefault()?.Description}");
                    }

                    var addLoginResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(externalProvider, externalUserId, externalProvider));
                    if (!addLoginResult.Succeeded)
                    {
                        throw new Exception($"Cannot add Google login: {addLoginResult.Errors.FirstOrDefault()?.Description}");
                    }
                }
                
            }


            await MapSessionCartToDatabaseCart(user.Id);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Sid, user.Id),
                new Claim("Provider", externalProvider),
            };

         
            var claimsIdentity = new ClaimsIdentity(authClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            //await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModelView payload)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(payload.Email);
                if (user != null && await _userManager.CheckPasswordAsync(user, payload.Password.Trim()))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Sid, user.Id),
                        
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var claimsIdentity = new ClaimsIdentity(authClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    await MapSessionCartToDatabaseCart(user.Id);

                    return RedirectToAction("Index", "Home");
                } else
                {
                    ModelState.AddModelError("Password", "Thông tin đăng nhập không chính xác");
                }
            } 
           

            return View(payload);
        }

        public IActionResult SignUp()
        {
            return View();
        }

        private async Task MapSessionCartToDatabaseCart(string userId)
        {
            CartSession cartSession = HttpContext.Session.Get<CartSession>("cart");

            if (cartSession == null)
                return;

            Cart? cart = await _storeDbContext.Carts
                .Include(cart => cart.CartItems)
                .SingleOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                cart.CartItems = new List<CartItem>();
                await _storeDbContext.Carts.AddAsync(cart);
            }
            else
            {
                _storeDbContext.CartItems.RemoveRange(cart.CartItems);
            }

            int quantity = 0;
            double totalPrice = 0;

            foreach (var item in cartSession.Items)
            {
                ProductVariant? productVariant = await _storeDbContext.ProductVariants
                    .Include(p => p.Product)
                    .ThenInclude(p => p.Images)
                    .Include(p => p.Color)
                    .Include(p => p.Size)
                    .SingleOrDefaultAsync(p => p.Id == item.ProductVariantId);

                CartItem cartItem = new CartItem
                {
                    ProductVariant = productVariant!,
                    ProductVariantId = item.ProductVariantId,
                    Cart = cart,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    SubTotal = item.SubTotal,
                };

                quantity++;
                totalPrice += item.SubTotal;

                cart.CartItems.Add(cartItem);
            }

            cart.Quantity = quantity;
            cart.TotalPrice = totalPrice;
            cart.UserId = userId;

            await _storeDbContext.SaveChangesAsync();
        }


        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpModelView payload)
        {
            if (ModelState.IsValid)
            {
                var appUser = new User
                {
                    FullName = payload.Fullname,
                    Email = payload.Email,
                    UserName = payload.Username,
                    PhoneNumber = "0000000000",
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(appUser, payload.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appUser, RoleConstant.ROLE_CUSTOMER);
                    return RedirectToAction(nameof(Login));
                }

            }

            return View(payload);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }
    }
}
