using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebBanQuanAo.Constants;
using WebBanQuanAo.Data;
using WebBanQuanAo.Models;
using WebBanQuanAo.Payload.Product;

namespace WebBanQuanAo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly StoreDbContext storeDbContext;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, StoreDbContext storeDbContext, UserManager<User> userManager)
        {
            _logger = logger;
            this.storeDbContext = storeDbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await storeDbContext.Products
                .Include(p => p.Evaluations)
                .Include(p => p.Images)
                .ToListAsync();

            products = products.Take(Math.Min(4, products.Count)).ToList();

            List<Product> favoriteProducts = await storeDbContext.Products
                .Include(p => p.Evaluations)
                .Where(p => p.Evaluations.Count > 0) 
                .OrderByDescending(p => p.Evaluations.Average(e => e.Stars)) 
                .ToListAsync();

            favoriteProducts = favoriteProducts.Take(Math.Min(4, favoriteProducts.Count)).ToList();

            var orderItems = await storeDbContext.OrderItems
                .Include(o => o.Order)
                .Include(oi => oi.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                .ToListAsync();

            var bestSellerProducts = orderItems
                .Where(o => o.Order.OrderStatus.Equals(OrderStatus.COMPLETED) || o.Order.OrderStatus.Equals(OrderStatus.DELIVERING))
                .GroupBy(oi => oi.ProductVariant.Product)
                .Select(g => new
                {
                    Product = g.Key,
                    TotalQuantitySold = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(g => g.TotalQuantitySold)
                .Select(g => g.Product)
                .ToList();

            bestSellerProducts = bestSellerProducts.Take(Math.Min(4, bestSellerProducts.Count)).ToList();

            ProductViewHome model = new ProductViewHome()
            {
                Products = products,
                FavoriteProducts = favoriteProducts,
                BestSellerProducts = bestSellerProducts
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
