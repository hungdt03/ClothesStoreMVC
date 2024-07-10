using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebBanQuanAo.Data;
using WebBanQuanAo.Models;
using WebBanQuanAo.Payload;

namespace WebBanQuanAo.Controllers
{
    public class ShopController : Controller
    {
        private readonly StoreDbContext _dbContext;
        private readonly ILogger<ShopController> _logger;   

        public ShopController(StoreDbContext dbContext, ILogger<ShopController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? categoryId, int pageIndex = 1, int pageSize = 20)
        {
            Category? category = null;
            List<Product> products = new List<Product>();
            var queryable = _dbContext.Products.AsQueryable();

            if (categoryId == null)
            {
                products = await queryable
                    .Include(p => p.Images)
                    .Include(p => p.ProductVariants)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            else
            {
                category = await _dbContext.Categories
                    .Include(c => c.Products)
                        .ThenInclude(p => p.Images)
                    .SingleOrDefaultAsync(c => c.Id == categoryId);

                if (category != null)
                {
                    products = category.Products
                         .Skip((pageIndex - 1) * pageSize)
                         .Take(pageSize)
                        .ToList();
                }
            }

            List<Brand> brands = await _dbContext.Brands.ToListAsync();
            List<Size> sizes = await _dbContext.Sizes.ToListAsync();
            List<Color> colors = await _dbContext.Colors.ToListAsync();

            var qr = (from c in _dbContext.Categories select c)
             .Include(c => c.ParentCategory)                
             .Include(c => c.CategoryChildren);            

            var categories = (await qr.ToListAsync())
                             .Where(c => c.ParentCategory == null)
                             .ToList();

            double maxPrice = _dbContext.Products.Max(p => p.Price);
            double minPrice = 0;

            ShopModelView payload = new ShopModelView()
            {
                Categories = categories,
                Category = category,
                Brands = brands,
                Sizes = sizes,
                Colors = colors,
                MaxPrice = maxPrice,
                MinPrice = minPrice,
                Products = products,
                SelectMinPrice = minPrice,
                SelectMaxPrice = maxPrice,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalRows = queryable.ToList(),
                TotalPages = (int)Math.Ceiling((double) queryable.ToList().Count / pageSize),
            };

            return View(payload);
        }

        [HttpPost]
        public async Task<IActionResult> Filter(FilterPayload payload, int pageIndex = 1, int pageSize=20)
        {
            Category? category = await _dbContext.Categories
                .Include(cate => cate.Products)
                .SingleOrDefaultAsync(c => c.Id == payload.CategoryId);

            IQueryable<Product> productsQuery = _dbContext.Products
                .Include(p => p.Brand)
                .Include(p => p.Images)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Color)
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Size)
                .AsQueryable();

            if (category != null)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == payload.CategoryId);
            }

            List<Product> products = await productsQuery
                .Where(p =>
                    p.Price >= payload.MinPrice && p.Price <= payload.MaxPrice
                    && (payload.BrandIds.Contains(p.BrandId.Value) || payload.BrandIds.Count == 0)
                    && (p.ProductVariants.Any(pv =>
                        (payload.SizeIds.Contains(pv.SizeId) || payload.SizeIds.Count == 0)
                        && (payload.ColorIds.Contains(pv.ColorId) || payload.ColorIds.Count == 0)
                    ) || p.ProductVariants.Count == 0)
                ).ToListAsync();

            var showProducts = products.Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize).ToList();

            var qr = (from c in _dbContext.Categories select c)
             .Include(c => c.ParentCategory)
             .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync())
                             .Where(c => c.ParentCategory == null)
                             .ToList();

            List<Brand> brands = await _dbContext.Brands.ToListAsync();
            List<Size> sizes = await _dbContext.Sizes.ToListAsync();
            List<Color> colors = await _dbContext.Colors.ToListAsync();

            double maxPrice = _dbContext.Products.Max(p => p.Price);
            double minPrice = 0;

            ShopModelView model = new ShopModelView()
            {
                Categories = categories,
                Category = category!,
                Brands = brands,
                Sizes = sizes,
                Colors = colors,
                MaxPrice = maxPrice,
                MinPrice = minPrice,
                Products = showProducts,
                SelectMaxPrice = payload.MaxPrice,
                SelectMinPrice = payload.MinPrice,
                CheckedBrandIds = payload.BrandIds,
                CheckedColorIds = payload.ColorIds,
                CheckedSizeIds = payload.SizeIds,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalRows = products,
                TotalPages = (int) Math.Ceiling((double)products.Count / pageSize),
            };

            return View("Index", model);
        }
    }
}
