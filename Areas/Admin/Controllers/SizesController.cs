using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Data;
using WebBanQuanAo.Helpers;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SizesController : AdminController
    {
        private readonly StoreDbContext _context;

        public SizesController(StoreDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Sizes
        public async Task<IActionResult> Index()
        {
              return _context.Sizes != null ? 
                          View(await _context.Sizes.ToListAsync()) :
                          Problem("Entity set 'StoreDbContext.Sizes'  is null.");
        }

        // GET: Admin/Sizes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Sizes == null)
            {
                return NotFound();
            }

            var size = await _context.Sizes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (size == null)
            {
                return NotFound();
            }

            return View(size);
        }

        // GET: Admin/Sizes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Sizes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ESize,MinHeight,MaxHeight,MinWeight,MaxWeight")] Size size)
        {
            if (ModelState.IsValid)
            {
                _context.Add(size);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(size);
        }

        // GET: Admin/Sizes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Sizes == null)
            {
                return NotFound();
            }

            var size = await _context.Sizes.FindAsync(id);
            if (size == null)
            {
                return NotFound();
            }
            return View(size);
        }

        // POST: Admin/Sizes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ESize,MinHeight,MaxHeight,MinWeight,MaxWeight")] Size size)
        {
            if (id != size.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(size);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SizeExists(size.Id))
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
            return View(size);
        }

        // GET: Admin/Sizes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Sizes == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Kích cỡ không tồn tại!", "Thất bại", NotificationType.Error);
                return RedirectToAction(nameof(Index));
            }
            var size = await _context.Sizes
                .Include(s => s.ProductVariants)
                .SingleOrDefaultAsync(s => s.Id == id);

            if (size != null)
            {
                if(size.ProductVariants == null || (size.ProductVariants != null && size.ProductVariants.Count == 0))
                {
                    _context.Sizes.Remove(size);
                    await _context.SaveChangesAsync();
                    NotificationsHelper.AddNotification(HttpContext, "Xóa kích cỡ thành công!", "Thành công", NotificationType.Success);
                } else
                {
                    NotificationsHelper.AddNotification(HttpContext, "Kích cỡ đang được liên kết với nhiều sản phẩm!", "Cảnh báo", NotificationType.Warning);
                }

                
            } else
            {
                NotificationsHelper.AddNotification(HttpContext, "Kích cỡ không tồn tại!", "Thất bại", NotificationType.Error);
            }

            
            return RedirectToAction(nameof(Index));
        }

        private bool SizeExists(int id)
        {
          return (_context.Sizes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
