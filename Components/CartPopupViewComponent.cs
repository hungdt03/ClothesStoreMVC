using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebBanQuanAo.Data;
using WebBanQuanAo.Extensions;
using WebBanQuanAo.Models;
using WebBanQuanAo.Payload.Sessions;

namespace WebBanQuanAo.Components
{
    public class CartPopupViewComponent : ViewComponent
    {
        private readonly StoreDbContext storeDbContext;
        private readonly UserManager<User> userManager;
        public CartPopupViewComponent(StoreDbContext storeDbContext, UserManager<User> userManager)
        {
            this.storeDbContext = storeDbContext;
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.User?.FindFirst(ClaimTypes.Sid)?.Value ?? "";
            var currentUser = await userManager.FindByIdAsync(userId);

            Cart cart = null;

            if (currentUser == null)
            {
                CartSession cartSession = GetCartFromSession();

                if (cartSession == null)
                {
                    return View(cart);
                }

                cart = await MapCartSessionToCart(cartSession);
            }
            else
            {
                cart = await GetCartFromDatabase(currentUser.Id);
            }

            return View(cart);
        }

        private async Task<Cart> MapCartSessionToCart(CartSession cartSession)
        {
            Cart cartResponse = new Cart();
            cartResponse.CartItems = new List<CartItem>();
            int quantity = 0;
            double totalPrice = 0;

            foreach (var item in cartSession.Items)
            {
                ProductVariant? productVariant = await storeDbContext.ProductVariants
                    .Include(p => p.Color)
                    .Include(p => p.Size)
                    .Include(p => p.Images)
                    .Include(x => x.Product)
                    .SingleOrDefaultAsync(p => p.Id == item.ProductVariantId);

                CartItem cartItem = new CartItem
                {
                    ProductVariant = productVariant!,
                    ProductVariantId = item.ProductVariantId,
                    Cart = cartResponse,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    SubTotal = item.SubTotal,
                };

                quantity++;
                totalPrice += item.SubTotal;

                cartResponse.CartItems.Add(cartItem);
            }

            cartResponse.Quantity = quantity;
            cartResponse.TotalPrice = totalPrice;

            return cartResponse;
        }

        private CartSession? GetCartFromSession()
        {
            return HttpContext.Session.Get<CartSession>("cart");
        }

        private async Task<Cart?> GetCartFromDatabase(string userId)
        {
            return await storeDbContext.Carts
               .Include(c => c.CartItems)
                   .ThenInclude(item => item.ProductVariant)
                       .ThenInclude(pv => pv.Color) // Load thuộc tính Color từ ProductVariant
                   .Include(c => c.CartItems)
                        .ThenInclude(item => item.ProductVariant)
                            .ThenInclude(pv => pv.Images)
                   .Include(c => c.CartItems)
                   .ThenInclude(item => item.ProductVariant)
                       .ThenInclude(pv => pv.Size) // Load thuộc tính Size từ ProductVariant
                   .Include(c => c.CartItems)
                   .ThenInclude(item => item.ProductVariant)
                       .ThenInclude(pv => pv.Product)
               .SingleOrDefaultAsync(c => c.UserId.Equals(userId));
        }


    }
}
