
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebBanQuanAo.Data;
using WebBanQuanAo.Extensions;
using WebBanQuanAo.Models;
using WebBanQuanAo.Payload.Cart;
using WebBanQuanAo.Payload.Sessions;

namespace WebBanQuanAo.Controllers
{
    //[Authorize]
    public class CartController : Controller
    {
        private readonly StoreDbContext storeDbContext;
        private readonly UserManager<User> userManager; 

        public CartController(StoreDbContext storeDbContext, UserManager<User> userManager)
        {
            this.storeDbContext = storeDbContext;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserName = User?.Identity?.Name ?? "";
            var currentUser = await userManager.FindByNameAsync(currentUserName);
            Cart cart = new Cart();
            

            if (currentUser == null)
            {
                CartSession cartSession = GetCartFromSession();

                if(cartSession == null)
                {
                    return View(null);
                }

                cart = await MapCartSessionToCart(cartSession);
            }
            else
            {
                cart = await GetCartFromDatabase(currentUser.Id);

            }
            


            return View(cart);
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
                        .ThenInclude(pv => pv.Size) // Load thuộc tính Size từ ProductVariant
                        .Include(c => c.CartItems)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(pv => pv.Images)
                    .Include(c => c.CartItems)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .SingleOrDefaultAsync(c => c.UserId.Equals(userId));
        }


        [HttpPost]
        public async Task<IActionResult> AddToCart(ProductDetailCart productDetailCart)
        {
            var userId = User?.FindFirst(ClaimTypes.Sid)?.Value ?? "";
            var currentUser = await userManager.FindByIdAsync(userId);
            ProductVariant? productVariant = await storeDbContext.ProductVariants
                .Include(p => p.Product)
                .SingleOrDefaultAsync(p => 
                    p.ProductId == productDetailCart.ProductId
                    && p.ColorId == productDetailCart.ColorId
                    && p.SizeId == productDetailCart.SizeId
                 );

            if (productVariant == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(userId))
            {
                var cartSession = HttpContext.Session.Get<CartSession>("cart");

                if (cartSession == null)
                    cartSession = CreateNewCartSession(productVariant, productDetailCart.Quantity);
                else
                {
                    AddOrUpdateCartItemSession(cartSession, productVariant, productDetailCart.Quantity);
                }

                HttpContext.Session.Set("cart", cartSession);
            }
            else
            {
                var cart = await storeDbContext.Carts
                    .Include(c => c.CartItems)
                    .SingleOrDefaultAsync(c => c.UserId.Equals(currentUser.Id));
                if (cart == null)
                    cart = CreateNewCartForUser(currentUser.Id, productVariant, productDetailCart.Quantity);
                else
                    AddOrUpdateCartItem(cart, productVariant, productDetailCart.Quantity);

                await storeDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private Cart CreateNewCart(ProductVariant productVariant, int qty = 1)
        {
            var cartItem = new CartItem
            {
                ProductVariant = productVariant,
                Quantity = qty,
                Price = productVariant.Product.Price,
                SubTotal = productVariant.Product.Price * qty
            };

            var newCart = new Cart
            {
                Quantity = 1,
                TotalPrice = productVariant.Product.Price * qty,
                CartItems = new List<CartItem> { cartItem }
            };

            return newCart;
        }

        // ==============START SESSION CART=============
        private CartSession CreateNewCartSession(ProductVariant productVariant, int qty = 1)
        {
            var cartItem = new CartItemSession
            {
                ProductVariantId = productVariant.Id,
                Quantity = qty,
                Price = productVariant.Product.Price,
                SubTotal = productVariant.Product.Price * qty
            };

            var newCart = new CartSession
            {
                TotalPrice = productVariant.Product.Price * qty,
                Items = new List<CartItemSession> { cartItem }
            };

            return newCart;
        }

        private void AddOrUpdateCartItemSession(CartSession cart, ProductVariant productVariant, int qty = 1)
        {
            var cartItem = cart.Items.SingleOrDefault(c => c.ProductVariantId == productVariant.Id);

            if (cartItem == null)
            {
                cartItem = new CartItemSession
                {
                    ProductVariantId = productVariant.Id,
                    Quantity = qty,
                    Price = productVariant.Product.Price,
                    SubTotal = productVariant.Product.Price * qty
                };

                cart.Items.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += qty;
                cartItem.SubTotal += productVariant.Product.Price * qty;
            }

            cart.TotalPrice += productVariant.Product.Price * qty;
        }

        // ==============END SESSION CART=============


        // ==============START USER CART=============

        private Cart CreateNewCartForUser(string userId, ProductVariant productVariant, int qty = 1)
        {
            var newCart = CreateNewCart(productVariant, qty);
            newCart.UserId = userId;
            storeDbContext.Add(newCart);
            return newCart;
        }

        private void AddOrUpdateCartItem(Cart cart, ProductVariant productVariant, int qty = 1)
        {
            var cartItem = cart.CartItems.SingleOrDefault(c => c.ProductVariantId == productVariant.Id);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ProductVariant = productVariant,
                    Cart = cart,
                    Quantity = qty,
                    Price = productVariant.Product.Price,
                    SubTotal = productVariant.Product.Price * qty
                };

                cart.CartItems.Add(cartItem);
                cart.Quantity += 1;
            }
            else
            {
                cartItem.Quantity += qty;
                cartItem.SubTotal += productVariant.Product.Price * qty;
            }


            cart.TotalPrice += productVariant.Product.Price * qty;
        }
        // ==============END USER CART=============



        [HttpPost]
        public async Task<IActionResult> Update([FromBody] CartItemPayload payload)
        {
            var userId = User?.FindFirst(ClaimTypes.Sid)?.Value ?? "";
            var currentUser = await userManager.FindByIdAsync(userId);
            ProductVariant? productVariant = await storeDbContext.ProductVariants
                .Include(p => p.Product)
                .SingleOrDefaultAsync(p => p.Id == payload.Id);

            if (currentUser == null)
            {
                var cartSession = HttpContext.Session.Get<CartSession>("cart");
                var cItem = cartSession.Items.SingleOrDefault(c => c.ProductVariantId == productVariant.Id);
                cItem.Quantity = payload.Quantity;
                cItem.SubTotal = productVariant.Product.Price * payload.Quantity;
                cartSession.TotalPrice = cartSession.Items.Sum(i => i.SubTotal);
                HttpContext.Session.Set("cart", cartSession);
                
                return Ok(MapCartResponse(await MapCartSessionToCart(cartSession)));
            }

            Cart? cart = await storeDbContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant.Product)
                .Include(c => c.CartItems)
                    .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant.Images)
                .Include(c => c.CartItems)
                    .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant.Color)
                .Include(c => c.CartItems)
                    .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant.Size)
                .SingleOrDefaultAsync(c => c.UserId.Equals(currentUser.Id));

            CartItem? cartItem = cart.CartItems
                    .SingleOrDefault(c => c.ProductVariantId == payload.Id);

            cartItem!.Quantity = payload.Quantity;
            cartItem.SubTotal = cartItem.Price * payload.Quantity;

            cart!.TotalPrice = cart.CartItems.Sum(c => c.SubTotal);
            await storeDbContext.SaveChangesAsync();

            return Ok(MapCartResponse(cart));
        }

        [HttpGet]
        public async Task<IActionResult> RemoveItem(int id)
        {
            var userId = User?.FindFirst(ClaimTypes.Sid)?.Value ?? "";
            var currentUser = await userManager.FindByIdAsync(userId);

            if (currentUser == null)
            {
                var cartSession = HttpContext.Session.Get<CartSession>("cart");

                CartItemSession? removeCartItem = cartSession.Items
                    .SingleOrDefault(c => c.ProductVariantId == id);

                if (removeCartItem != null)
                {
                    cartSession.TotalPrice -= removeCartItem.SubTotal;
                    cartSession.Items.Remove(removeCartItem);
                }
                    
                
                HttpContext.Session.Set("cart", cartSession);
            }
            else
            {
                var cart = await storeDbContext.Carts
                    .Include(c => c.CartItems)
                    .SingleOrDefaultAsync(c => c.UserId.Equals(currentUser.Id));

                CartItem? removeCartItem = cart.CartItems
                    .SingleOrDefault(p => p.ProductVariantId == id);

                if (removeCartItem != null)
                {
                    cart.TotalPrice -= removeCartItem.SubTotal;
                    cart.CartItems.Remove(removeCartItem);
                }
                await storeDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
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
                    .Include(p => p.Product)
                    .Include(p => p.Images)
                    .Include(p => p.Color)
                    .Include(p => p.Size)
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

        private object MapCartResponse(Cart cart)
        {
            var cartItems = cart.CartItems.Select(item => new
            {
                Id = item.Id,
                ProductVariantId = item.ProductVariantId,
                Quantity = item.Quantity,
                SubTotal = item.SubTotal,
                ProductVariant = new
                {
                    Id = item.ProductVariant.Id,
                    Name = item.ProductVariant.Product.Name,
                    Price = item.ProductVariant.Product.Price,
                    Images = item.ProductVariant.Images != null ?  item.ProductVariant.Images.Select(image => new
                    {
                        Id = image.Id,
                        Url = image.Url,
                    }).ToList() : null,
                    Color = item.ProductVariant.Color != null ? new
                    {
                        Id = item.ProductVariant.Color.Id,
                        Name = item.ProductVariant.Color.Name
                    } : null,
                    Size = item.ProductVariant.Size != null ? new
                    {
                        Id = item.ProductVariant.Size.Id,
                        ESize = item.ProductVariant.Size.ESize
                    } : null,

                }
            }).ToList();

            return new
            {
                Id = cart.Id,
                TotalPrice = cart.TotalPrice,
                CartItems = cartItems,
                Quantity = cartItems.Count,
            };
        }
    }
}
