using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Data;
using WebBanQuanAo.Helper;
using WebBanQuanAo.Helpers;
using WebBanQuanAo.Models;
using WebBanQuanAo.Payload.Pagination;
using WebBanQuanAo.Payload.Product.Variant;

namespace WebBanQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductVariantController : AdminController
    {
        private readonly StoreDbContext storeDbContext;
        private readonly ILogger<ProductVariantController> logger; 
        private readonly UploadFileHelper uploadFileHelper;

        public ProductVariantController(StoreDbContext storeDbContext, ILogger<ProductVariantController> logger, UploadFileHelper uploadFileHelper)
        {
            this.storeDbContext = storeDbContext;
            this.logger = logger;
            this.uploadFileHelper = uploadFileHelper;
        }

        public async Task<IActionResult> Index(int pageSize = 8, int pageIndex = 1, string query = "")
        {
            IQueryable<ProductVariant> queryable = storeDbContext.ProductVariants
                .Include(p => p.Product)
                .Include(p => p.Color)
                .Include(p => p.Size);

            if (!string.IsNullOrEmpty(query))
            {
                string lowerQuery = query.ToLower();
                queryable = queryable.Where(c => c.Product.Name.ToLower().Contains(lowerQuery));
            }

            List<ProductVariant> items = await queryable
                .OrderByDescending(s => s.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            PaginationModelView<ProductVariant> modelView = new PaginationModelView<ProductVariant>()
            {
                TotalRows = await queryable.ToListAsync(),
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return View(modelView);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["ColorId"] = new SelectList(storeDbContext.Colors, "Id", "Name");
            ViewData["ProductId"] = new SelectList(storeDbContext.Products, "Id", "Name");
            ViewData["SizeId"] = new SelectList(storeDbContext.Sizes, "Id", "ESize");
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVariantModelView modelView)
        {

            if (ModelState.IsValid)
            {
                ProductVariant? checkExisted = await storeDbContext.ProductVariants
                    .SingleOrDefaultAsync(p => 
                        p.ColorId == modelView.ColorId
                        && p.SizeId == modelView.SizeId
                        && p.ProductId == modelView.ProductId
                    );

                if (checkExisted != null)
                {
                    checkExisted.InStock += modelView.InStock;
                } else
                {
                    ProductVariant newProductVariant = new ProductVariant();
                    newProductVariant.ProductId = modelView.ProductId;
                    newProductVariant.SizeId = modelView.SizeId;
                    newProductVariant.ColorId = modelView.ColorId;
                    newProductVariant.InStock = modelView.InStock;
                    newProductVariant.IsActive = true;
                    List<ProductVariantImage> images = new List<ProductVariantImage>();
                    try
                    {
                        var imgUrls = await uploadFileHelper.UploadImagesAsync(modelView.Images);
                        foreach(var imgUrl in imgUrls) { 
                            var pVariantImg = new ProductVariantImage();
                            pVariantImg.Url = imgUrl;

                            images.Add(pVariantImg);
                        }

                    } catch (Exception ex) 
                    {
                        ModelState.AddModelError("Images", ex.Message);
                        ViewData["ColorId"] = new SelectList(storeDbContext.Colors, "Id", "Name");
                        ViewData["ProductId"] = new SelectList(storeDbContext.Products, "Id", "Name");
                        ViewData["SizeId"] = new SelectList(storeDbContext.Sizes, "Id", "ESize");
                        return View(modelView);
                    }

                    newProductVariant.Images = images;
                    await storeDbContext.ProductVariants.AddAsync(newProductVariant);
                    
                }

                await storeDbContext.SaveChangesAsync();
                NotificationsHelper.AddNotification(HttpContext, "Thêm sản phẩm thành công!", "Thành công", NotificationType.Success);
                return RedirectToAction(nameof(Index));
            }


            ViewData["ColorId"] = new SelectList(storeDbContext.Colors, "Id", "Name");
            ViewData["ProductId"] = new SelectList(storeDbContext.Products, "Id", "Name");
            ViewData["SizeId"] = new SelectList(storeDbContext.Sizes, "Id", "ESize");
            return View(modelView);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Sản phẩm không có sẵn!", "Thất bại", NotificationType.Error);
            }

            ProductVariant? productVariant = await storeDbContext.ProductVariants
               .SingleOrDefaultAsync(p => p.Id == id);

            if (productVariant == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Sản phẩm không có sẵn!", "Thất bại", NotificationType.Error);
            }

            ViewData["ColorId"] = new SelectList(storeDbContext.Colors, "Id", "Name");
            ViewData["ProductId"] = new SelectList(storeDbContext.Products, "Id", "Name");
            ViewData["SizeId"] = new SelectList(storeDbContext.Sizes, "Id", "ESize");

            EditVariantModelView model = new EditVariantModelView();
            model.ProductId = productVariant.ProductId;
            model.ColorId = productVariant.ColorId;
            model.SizeId = productVariant.SizeId;
            model.InStock = productVariant.InStock;

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditVariantModelView modelView)
        {
            if (id == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Sản phẩm không có sẵn!", "Thất bại", NotificationType.Error);
            }

            ProductVariant? productVariant = await storeDbContext.ProductVariants
               .SingleOrDefaultAsync(p => p.Id == id);

            if (productVariant == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Sản phẩm không có sẵn!", "Thất bại", NotificationType.Error);
            }

            if (!ModelState.IsValid) {
                ViewData["ColorId"] = new SelectList(storeDbContext.Colors, "Id", "Name");
                ViewData["ProductId"] = new SelectList(storeDbContext.Products, "Id", "Name");
                ViewData["SizeId"] = new SelectList(storeDbContext.Sizes, "Id", "ESize");

                return View(modelView);
            }

            productVariant.SizeId = modelView.SizeId;
            productVariant.ColorId = modelView.ColorId;
            productVariant.InStock = modelView.InStock;
            productVariant.ProductId = modelView.ProductId;

            await storeDbContext.SaveChangesAsync();
            NotificationsHelper.AddNotification(HttpContext, "Cập nhật sản phẩm thành công!", "Thành công", NotificationType.Success);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int? id)
        {
            ProductVariant? productVariant = await storeDbContext.ProductVariants
                .Include(p => p.Color)
                .Include(p => p.Size)
                .Include(p => p.Product)
                    .ThenInclude(p => p.Category)
                .Include(p => p.Product)
                    .ThenInclude(p => p.Brand)
                .SingleOrDefaultAsync(p => p.Id == id);

            if(productVariant == null)
            {
                return NotFound();
            }

            return View(productVariant);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || storeDbContext.Products == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Sản phẩm không có sẵn!", "Thất bại", NotificationType.Error);
                return RedirectToAction(nameof(Index));
            }

            var product = await storeDbContext.ProductVariants
                .Include(p => p.OrderItems)
                .Include(p => p.CartItems)
                .SingleOrDefaultAsync(product => product.Id == id);

            if (product != null)
            {
                if (product.OrderItems == null || (product.OrderItems != null && product.OrderItems.Count == 0))
                {
                    if (product.CartItems == null || (product.CartItems != null && product.CartItems.Count == 0))
                    {
                        storeDbContext.ProductVariants.Remove(product);
                        await storeDbContext.SaveChangesAsync();
                        NotificationsHelper.AddNotification(HttpContext, "Xóa sản phẩm thành công!", "Thành công", NotificationType.Success);
                    } else
                    {
                        NotificationsHelper.AddNotification(HttpContext, "Không thể xóa sản phẩm hiện tại!", "Cảnh báo", NotificationType.Warning);
                    }

                }
                else
                {
                    NotificationsHelper.AddNotification(HttpContext, "Sản phẩm không thể xóa hiện tại", "Cảnh báo", NotificationType.Warning);
                }

            }
            else
            {
                NotificationsHelper.AddNotification(HttpContext, "Sản phẩm không tồn tại!", "Thất bại", NotificationType.Error);
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
