using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebBanQuanAo.Data;
using WebBanQuanAo.Helpers;
using WebBanQuanAo.Models;
using WebBanQuanAo.Payload.Pagination;

namespace WebBanQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : AdminController
    {
        private readonly StoreDbContext storeDbContext;
        private readonly ILogger<CategoryController> logger;

        public CategoryController(StoreDbContext storeDbContext, ILogger<CategoryController> logger)
        {
            this.storeDbContext = storeDbContext;
            this.logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> Index(int pageSize = 8, int pageIndex = 1, string query = "")
        {
            IQueryable<Category> queryable = storeDbContext.Categories; 

            if (!string.IsNullOrEmpty(query))
            {
                string lowerQuery = query.ToLower();
                queryable = queryable.Where(c => c.Name.ToLower().Contains(lowerQuery));
            }

            List<Category> items = await queryable
                .OrderByDescending(s => s.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            PaginationModelView<Category> modelView = new PaginationModelView<Category>()
            {
                TotalRows = await queryable.ToListAsync(), 
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return View(modelView);
        }

        private async Task<IEnumerable<Category>> GetItemsSelectCategories()
        {

            var qr = (from c in storeDbContext.Categories select c)
           .Include(c => c.ParentCategory)
           .Include(c => c.CategoryChildren);

            var categories = (await qr.ToListAsync())
                             .Where(c => c.ParentCategory == null)
                             .ToList();


            List<Category> resultItems = new List<Category>() {
                new Category() {
                    Id = -1,
                    Name = "Không có danh mục cha"
                }
            };

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

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            ViewData["ParentId"] = new SelectList(await GetItemsSelectCategories(), "Id", "Name", -1);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("ParentCategoryId,Name,Description")]Category category)
        {
            if(ModelState.IsValid)
            {
                if (category.ParentCategoryId == -1)
                    category.ParentCategoryId = null;

                await storeDbContext.Categories.AddAsync(category);
                await storeDbContext.SaveChangesAsync();
                NotificationsHelper.AddNotification(HttpContext, "Danh mục đã được thêm thành công!", "Thành công", NotificationType.Success);
                return RedirectToAction(nameof(Index));
            }

            
            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            Category? checkCategory = await storeDbContext.Categories.FindAsync(id);

            if (checkCategory == null)
            {
                return NotFound();
            }


            return View(checkCategory);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Name, Description")] Category category)
        {
            Category? checkCategory = await storeDbContext.Categories.FindAsync(id);

            if(checkCategory == null)
            {
                return NotFound();
            }

            checkCategory.Name = category.Name;
            checkCategory.Description = category.Description;
            await storeDbContext.SaveChangesAsync();
            NotificationsHelper.AddNotification(HttpContext, "Danh mục đã được cập nhật thành công!", "Thành công", NotificationType.Success);
            return RedirectToAction(nameof(Index)); 
        }

        public async Task<IActionResult> Details(int id)
        {
            Category? checkCategory = await storeDbContext.Categories.FindAsync(id);

            if (checkCategory == null)
            {
                return NotFound();
            }

            return View(checkCategory);
        }

        public async Task<IActionResult> Delete(int id)
        {
            Category? checkCategory = await storeDbContext.Categories
                .Include(c => c.Products)
                .SingleOrDefaultAsync(c => c.Id == id);

            if (id == null || checkCategory == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Danh mục không tồn tại!", "Thất bại", NotificationType.Error);
                return RedirectToAction(nameof(Index));
            } 

            if(checkCategory != null)
            {
                if(checkCategory.Products == null || (checkCategory.Products != null && checkCategory.Products.Count == 0))
                {
                    storeDbContext.Categories.Remove(checkCategory);
                    await storeDbContext.SaveChangesAsync();
                    NotificationsHelper.AddNotification(HttpContext, "Xóa danh mục thành công!", "Thành công", NotificationType.Success);
                } else
                {
                    NotificationsHelper.AddNotification(HttpContext, "Danh mục đang được liên kết với nhiều sản phẩm", "Cảnh báo", NotificationType.Warning);
                }
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
