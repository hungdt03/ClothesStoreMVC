using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using WebBanQuanAo.Data;
using WebBanQuanAo.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using WebBanQuanAo.Payload.Pagination;
using WebBanQuanAo.Helpers;

namespace WebBanQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandsController : AdminController
    {
        private readonly StoreDbContext _context;

        public BrandsController(StoreDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Brands
        public async Task<IActionResult> Index(int pageSize = 8, int pageIndex = 1, string query = "")
        {
            IQueryable<Brand> queryable = _context.Brands;

            if (!string.IsNullOrEmpty(query))
            {
                string lowerQuery = query.ToLower();
                queryable = queryable.Where(c => c.Name.ToLower().Contains(lowerQuery));
            }

            List<Brand> items = await queryable
                .OrderByDescending(s => s.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            PaginationModelView<Brand> modelView = new PaginationModelView<Brand>()
            {
                TotalRows = await queryable.ToListAsync(),
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return View(modelView);
        }

        // GET: Admin/Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Brands == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // GET: Admin/Brands/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Brands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brand);
                await _context.SaveChangesAsync();
                NotificationsHelper.AddNotification(HttpContext, "Thương hiệu đã được thêm thành công!", "Thành công", NotificationType.Success);
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        // GET: Admin/Brands/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Brands == null)
            {
                return NotFound();
            }

            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        // POST: Admin/Brands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Brand brand)
        {
            if (id != brand.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brand);
                    await _context.SaveChangesAsync();
                    NotificationsHelper.AddNotification(HttpContext, "Cập nhật thương hiệu thành công!", "Thành công", NotificationType.Success);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandExists(brand.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        // GET: Admin/Brands/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Brands == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Xóa thương hiệu thất bại!", "Thất bại", NotificationType.Error);
                return RedirectToAction(nameof(Index));
            }
            var brand = await _context.Brands
                .Include(br => br.Products)
                .SingleOrDefaultAsync(br => br.Id == id);

            if (brand != null)
            {
                if (brand.Products == null || (brand.Products != null && brand.Products.Count == 0))
                {
                    _context.Brands.Remove(brand);
                    await _context.SaveChangesAsync();
                    NotificationsHelper.AddNotification(HttpContext, "Xóa thương hiệu thành công!", "Thành công", NotificationType.Success);
                } else
                {
                    NotificationsHelper.AddNotification(HttpContext, "Thương hiệu đang được liên kết với nhiều sản phẩm!", "Cảnh báo", NotificationType.Warning);
                }
               
            } else
            {
                NotificationsHelper.AddNotification(HttpContext, "Thương hiệu không tồn tại!", "Thất bại", NotificationType.Error);
            }

            return RedirectToAction(nameof(Index));
        }


        private bool BrandExists(int id)
        {
          return (_context.Brands?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
