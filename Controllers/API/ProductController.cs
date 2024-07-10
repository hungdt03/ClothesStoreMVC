using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Data;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly StoreDbContext storeDbContext;

        public ProductController(StoreDbContext storeDbContext)
        {
            this.storeDbContext = storeDbContext;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            Product? product = await storeDbContext.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .SingleOrDefaultAsync(p => p.Id == id);

            var colors = await storeDbContext.ProductVariants
                .Where(pv => pv.IsActive && pv.ProductId == id)
                .GroupBy(pv => pv.ColorId)
                .Select(g => g.First().Color)
                .ToListAsync();

            var sizes = await storeDbContext.ProductVariants
                .Where(pv => pv.IsActive && pv.ProductId == id)
                .GroupBy(pv => pv.SizeId)
                .Select(g => g.First().Size)
                .ToListAsync();

            var payload = new
            {
                Product = MapProductResponse(product),
                Sizes = sizes,
                Colors = colors
            };

            return Ok(payload);
        }

        private object MapProductResponse(Product product)
        {
            return new
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Thumbnail = product.Thumbnail,
                ZoomImage = product.ZoomImage,
                Images = product.Images != null ? product.Images.Select(i => i.Url).ToList() : null,
                Category = product.Category != null ? new
                {
                    Id = product.CategoryId,
                    Name = product.Category.Name,   
                } : null,
                Brand = product.Brand != null ? new
                {
                    Id = product.BrandId,
                    Name = product.Brand.Name,
                } : null,
            };
        }
    }
}
