using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebBanQuanAo.Data;
using WebBanQuanAo.Helper;
using WebBanQuanAo.Helpers;
using WebBanQuanAo.Models;
using WebBanQuanAo.Payload.Pagination;
using WebBanQuanAo.Payload.Product;

namespace WebBanQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : AdminController
    {
        private readonly StoreDbContext _context;
        private readonly ILogger<ProductsController> _logger;
        private readonly UploadFileHelper uploadFileHelper;

        public ProductsController(StoreDbContext context, ILogger<ProductsController> logger, UploadFileHelper uploadFileHelper)
        {
            _context = context;
            _logger = logger;
            this.uploadFileHelper = uploadFileHelper;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index(int pageSize = 8, int pageIndex = 1, string query = "")
        {
            IQueryable<Product> queryable = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category);

            if (!string.IsNullOrEmpty(query))
            {
                string lowerQuery = query.ToLower();
                queryable = queryable.Where(c => c.Name.ToLower().Contains(lowerQuery));
            }

            List<Product> items = await queryable
                .OrderByDescending(s => s.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            PaginationModelView<Product> modelView = new PaginationModelView<Product>()
            {
                TotalRows = await queryable.ToListAsync(),
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return View(modelView);
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        private async Task<IEnumerable<Category>> GetItemsSelectCategories()
        {
            var qr = (from c in _context.Categories select c)
            .Include(c => c.ParentCategory)
            .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync())
                             .Where(c => c.ParentCategory == null)
                             .ToList();

            List<Category> resultItems = new List<Category>();

            Action<List<Category>, int> _ChangeTitleCategory = null;
            Action<List<Category>, int> ChangeTitleCategory = (items, level) => {
                string prefix = string.Concat(Enumerable.Repeat("—", level));
                foreach (var item in items)
                {
                    item.Name = prefix + " " + item.Name;
                    resultItems.Add(item);
                    if ((item.CategoryChildren != null) && (item.CategoryChildren.Count > 0))
                    {
                        _ChangeTitleCategory(item.CategoryChildren.ToList(), level + 1);
                    }
                }
            };

            _ChangeTitleCategory = ChangeTitleCategory;
            ChangeTitleCategory(categories, 0);

            return resultItems;
        }

        // GET: Admin/Products/Create
        public async Task<IActionResult> Create()
        {

            ViewData["CategoryId"] = new SelectList(await GetItemsSelectCategories(), "Id", "Name", 0);
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name");
            return View();
        }

        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel product)
        {
            if (ModelState.IsValid)
            {
                Product newProduct = new Product();
                newProduct.Name = product.Name;
                newProduct.Description = product.Description;
                newProduct.Price = product.Price;
                newProduct.CategoryId = product.CategoryId;
                newProduct.BrandId = product.BrandId;

                try
                {
                    string thumbnail = await uploadFileHelper.UploadImageAsync(product.Thumbnail);
                    newProduct.Thumbnail = thumbnail;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Thumbnail", ex.Message);
                    ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Id", product.BrandId);
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Description", product.CategoryId);
                    return View(product);
                }

                try
                {
                    string zoomImage = await uploadFileHelper.UploadImageAsync(product.ZoomImage);
                    newProduct.ZoomImage = zoomImage;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ZoomImage", ex.Message);
                    ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Id", product.BrandId);
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Description", product.CategoryId);
                    return View(product);
                }


                ICollection<string> imageUrls = new List<string>();

                try
                {
                    imageUrls = await uploadFileHelper.UploadImagesAsync(product.Images);
                }
                catch (Exception ex)
                {
                    ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Id", product.BrandId);
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Description", product.CategoryId);
                    return View(product);
                }

                ICollection<Image> images = new List<Image>();

                foreach (var url in imageUrls)
                {
                    images.Add(new Image()
                    {
                        Url = url,
                    });

                }

                newProduct.Images = images;
                _context.Products.Add(newProduct);
                await _context.SaveChangesAsync();
                NotificationsHelper.AddNotification(HttpContext, "Sản phẩm đã được thêm thành công!", "Thành công", NotificationType.Success);
                return RedirectToAction(nameof(Index));
            }


            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Id", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Description", product.CategoryId);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Images)
                .SingleOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);

            ProductViewModel viewModel = new ProductViewModel();
            viewModel.Name = product.Name;
            viewModel.Description = product.Description;
            viewModel.Price = product.Price;

            return View(viewModel);
        }

        // POST: Admin/Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel product)
        {

            if (ModelState.IsValid)
            {
                var updatedProduct = await _context.Products
                    .Include(p => p.Images)
                    .SingleOrDefaultAsync(p => p.Id == id);

                if (updatedProduct == null)
                    NotificationsHelper.AddNotification(HttpContext, "Sản phẩm không có sẵn!", "Thất bại", NotificationType.Error);


                updatedProduct.Name = product.Name;
                updatedProduct.Description = product.Description;
                updatedProduct.BrandId = product.BrandId;
                updatedProduct.CategoryId = product.CategoryId;


                await _context.SaveChangesAsync();
                NotificationsHelper.AddNotification(HttpContext, "Cập nhật sản phẩm thành công!", "Thành công", NotificationType.Success);
                return RedirectToAction(nameof(Index));
            }

            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeThumbnail(int id, IFormFile productThumbnail)
        {
            if (productThumbnail != null)
            {
                Product? product = await _context.Products.FindAsync(id); 

                if(product == null)
                {
                    NotificationsHelper.AddNotification(HttpContext, "Sản phẩm không tồn tại", "Thất bại", NotificationType.Error);
                    return RedirectToAction(nameof(Details), new { id = id });
                } 
                
                string url = await uploadFileHelper.UploadImageAsync(productThumbnail);
                product.Thumbnail = url;
                await _context.SaveChangesAsync();
                NotificationsHelper.AddNotification(HttpContext, "Cập nhật ảnh sản phẩm thành công", "Thành công", NotificationType.Success);
            }
            else
            {
                NotificationsHelper.AddNotification(HttpContext, "Chưa file nào được tải lên", "Cảnh báo", NotificationType.Warning);
            }

            
            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> AddImageProduct(int id, List<IFormFile> images)
        {
            if (images != null && images.Count > 0)
            {
                Product? product = await _context.Products
                    .Include(p => p.Images)
                    .SingleOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    NotificationsHelper.AddNotification(HttpContext, "Sản phẩm không tồn tại", "Thất bại", NotificationType.Error);
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                try
                {
                    List<string> urls = await uploadFileHelper.UploadImagesAsync(images);
                    foreach (var item in urls)
                    {
                        Image img = new Image()
                        {
                            Url = item,
                            ProductId = id
                        };

                        product?.Images?.Add(img);
                    }

                    await _context.SaveChangesAsync();
                    NotificationsHelper.AddNotification(HttpContext, "Thêm các ảnh sản phẩm thành công", "Thành công", NotificationType.Success);
                } catch(Exception e)
                {
                    NotificationsHelper.AddNotification(HttpContext, e.Message, "Thất bại", NotificationType.Error);
                }

               
            }
            else
            {
                NotificationsHelper.AddNotification(HttpContext, "Chưa file nào được tải lên", "Cảnh báo", NotificationType.Warning);
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveImageProduct(int imageId, int productId)
        {
            Image? image = await _context.Images.FindAsync(imageId);

            if(image == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Hình ảnh không có sẵn!", "Thất bại", NotificationType.Error);
                return RedirectToAction(nameof(Details), new { id = productId });
            }

            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
            NotificationsHelper.AddNotification(HttpContext, "Xóa ảnh thành công!", "Thành công", NotificationType.Success);
            return RedirectToAction(nameof(Details), new { id = productId });
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Sản phẩm không có sẵn!", "Thất bại", NotificationType.Error);
                return RedirectToAction(nameof(Index));
            }

            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.ProductVariants)
                .SingleOrDefaultAsync(product => product.Id == id);

            if (product != null)
            {
                if (product.ProductVariants == null || (product.ProductVariants != null && product.ProductVariants.Count == 0))
                {
                    if (product.Images != null)
                    {
                        _context.Images.RemoveRange(product.Images);
                    }

                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                    NotificationsHelper.AddNotification(HttpContext, "Xóa sản phẩm thành công!", "Thành công", NotificationType.Success);
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



        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }


    }
}
