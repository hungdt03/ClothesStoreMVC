using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Data;
using WebBanQuanAo.Extensions;
using WebBanQuanAo.Helper;
using WebBanQuanAo.Helpers;
using WebBanQuanAo.Models;
using WebBanQuanAo.Payload.Product;

namespace WebBanQuanAo.Controllers
{
    public class ProductController : Controller
    {
        private readonly StoreDbContext storeDbContext;
        private readonly UploadFileHelper uploadFileHelper;

        public ProductController(StoreDbContext storeDbContext, UploadFileHelper uploadFileHelper)
        {
            this.storeDbContext = storeDbContext;
            this.uploadFileHelper = uploadFileHelper;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Details(int id)
        {

            Product? product = await storeDbContext.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductVariants)
                    .ThenInclude(p => p.Color)
                .Include(p => p.ProductVariants)
                    .ThenInclude(p => p.Size)
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

            var reviews = await storeDbContext.Evaluations
                .Include(r => r.User)
                .Include(r => r.Images)
                .OrderByDescending(p => p.Id)
                .Where(p => p.ProductId == id)
                .ToListAsync();

            var payload = new ProductDetails
            {
                Reviews = reviews,
                Product = product,
                Sizes = sizes,
                Colors = colors
            };

            if (product == null)
            {
                return NotFound();
            }

            return View(payload);
        }

        [HttpPost]
        public async Task<IActionResult> Rating(int? id, ProductEvaluation model)
        {

            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;

                NotificationsHelper.AddNotification(HttpContext, firstError ?? "Có lỗi xảy ra", "Thất bại", NotificationType.Error);
                return RedirectToAction("OrderHistory", "Order"); 
            }

            Evaluation evaluation = new Evaluation();
            string userId = HttpContext.User.GetUserId();
            evaluation.Images = new List<ImageEvaluation>();
            evaluation.Stars = model.Star;
            evaluation.Content = model.Content;
            evaluation.ProductId = id.Value;
            evaluation.UserId = userId;
            evaluation.DateCreated = DateTime.Now;
            evaluation.Favorites = 0;

            try
            {
                List<string> imageUrls = await uploadFileHelper.UploadImagesAsync(model.Images);

                foreach(var item in imageUrls)
                {
                    ImageEvaluation img = new ImageEvaluation();
                    img.Url = item;
                    evaluation.Images.Add(img);
                }

                await storeDbContext.Evaluations.AddAsync(evaluation);
                await storeDbContext.SaveChangesAsync();
                NotificationsHelper.AddNotification(HttpContext, "Đánh giá thành công", "Thành công", NotificationType.Success);

            } catch
            {
                NotificationsHelper.AddNotification(HttpContext, "Có lỗi xảy ra", "Thất bại", NotificationType.Error);
            }

            return RedirectToAction("OrderHistory", "Order");
        }
       

        [HttpPost]
        public async Task<IActionResult> GetSizesByColor([FromBody] ProductOptionPayload payload)
        {
            var sizes = await storeDbContext.ProductVariants
                .Include(p => p.Size)
                .Where(p =>
                    p.ProductId == payload.ProductId
                    && p.ColorId == payload.ColorId
                )
                .Select(p => p.Size)
                .Distinct()
                .ToListAsync();

            var images = await storeDbContext.ProductVariants
                .Where(p =>
                    p.ProductId == payload.ProductId
                    && p.ColorId == payload.ColorId
                )
                .SelectMany(p => p.Images)
                .ToListAsync();

            var response = new
            {
                Sizes = sizes,
                Images = images
            };

            return Ok(response);
        }
    }
}
