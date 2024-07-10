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
    public class ColorsController : AdminController
    {
        private readonly StoreDbContext _context;

        public ColorsController(StoreDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Colors
        public async Task<IActionResult> Index()
        {
              return _context.Colors != null ? 
                          View(await _context.Colors.ToListAsync()) :
                          Problem("Entity set 'StoreDbContext.Colors'  is null.");
        }

        // GET: Admin/Colors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Colors == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Màu sắc không tồn tại!", "Thất bại", NotificationType.Error);
            }

            var color = await _context.Colors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (color == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Màu sắc không tồn tại!", "Thất bại", NotificationType.Error);
            }

            return View(color);
        }

        // GET: Admin/Colors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Colors/Create
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,HexCode")] Color color)
        {
            if (ModelState.IsValid)
            {
                _context.Add(color);
                await _context.SaveChangesAsync();
                NotificationsHelper.AddNotification(HttpContext, "Màu sắc đã được thêm thành công!", "Thành công", NotificationType.Success);
                return RedirectToAction(nameof(Index));
            }
            return View(color);
        }

        // GET: Admin/Colors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Colors == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Màu sắc không tồn tại!", "Thất bại", NotificationType.Error);
            }

            var color = await _context.Colors.FindAsync(id);
            if (color == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Màu sắc không tồn tại!", "Thất bại", NotificationType.Error);
            }
            return View(color);
        }

        // POST: Admin/Colors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,HexCode")] Color color)
        {
            if (id != color.Id)
            {
                NotificationsHelper.AddNotification(HttpContext, "Màu sắc không tồn tại!", "Thất bại", NotificationType.Error);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(color);
                    await _context.SaveChangesAsync();
                    NotificationsHelper.AddNotification(HttpContext, "Màu sắc đã được cập nhật thành công!", "Thành công", NotificationType.Success);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ColorExists(color.Id))
                    {
                        NotificationsHelper.AddNotification(HttpContext, "Màu sắc không tồn tại!", "Thất bại", NotificationType.Error);
                    }
                    else
                    {
                        throw;
                    }
                }

               
                return RedirectToAction(nameof(Index));
            }
            return View(color);
        }

        // GET: Admin/Colors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null ||_context.Colors == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Màu sắc không tồn tại!", "Thất bại", NotificationType.Error);
                return RedirectToAction(nameof(Index));
            }

            var color = await _context.Colors
                .Include(c => c.ProductVariants)
                .SingleOrDefaultAsync(c => c.Id == id);

            if (color != null)
            {

                if (color.ProductVariants == null || (color.ProductVariants != null && color.ProductVariants.Count == 0))
                {
                    _context.Colors.Remove(color);
                    await _context.SaveChangesAsync();
                    NotificationsHelper.AddNotification(HttpContext, "Màu sắc đã được xóa thành công!", "Thành công", NotificationType.Success);
                }
                else
                    NotificationsHelper.AddNotification(HttpContext, "Màu sắc đang được liên kết với nhiều sản phẩm!", "Cảnh báo", NotificationType.Warning);

            } else
            {
                NotificationsHelper.AddNotification(HttpContext, "Màu sắc không tồn tại!", "Thất bại", NotificationType.Error);
            }
            
            return RedirectToAction(nameof(Index));
        }

       

        private bool ColorExists(int id)
        {
          return (_context.Colors?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
