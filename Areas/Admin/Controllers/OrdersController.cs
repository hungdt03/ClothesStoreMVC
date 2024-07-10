using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanQuanAo.Constants;
using WebBanQuanAo.Data;
using WebBanQuanAo.Helpers;
using WebBanQuanAo.Models;
using WebBanQuanAo.Payload.Pagination;

namespace WebBanQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : AdminController
    {
        private readonly StoreDbContext storeDbContext;

        public OrdersController(StoreDbContext storeDbContext)
        {
            this.storeDbContext = storeDbContext;
        }

        public async Task<IActionResult> Index(int pageSize = 8, int pageIndex = 1, string query="")
        {
            IQueryable<Order> queryable = storeDbContext.Orders
                .Include(o => o.OrderInfo)
                .Include(o => o.OrderItems);

            if (!string.IsNullOrEmpty(query))
            {
                string lowerQuery = query.ToLower();
                queryable = queryable.Where(c => (c.OrderInfo.FirstName + " " + c.OrderInfo.LastName).ToLower().Contains(lowerQuery));
            }

            List<Order> items = await queryable
                .OrderByDescending(s => s.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            PaginationModelView<Order> modelView = new PaginationModelView<Order>()
            {
                TotalRows = await queryable.ToListAsync(),
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return View(modelView);
        }

        public async Task<IActionResult> Details(int? id)
        {
            Order? order = await storeDbContext.Orders
                .Include(o => o.OrderInfo)
                .Include(o => o.Payment)
                .Include(o => o.OrderItems)
                    .ThenInclude(o => o.ProductVariant)
                        .ThenInclude(o => o.Product)
                            .ThenInclude(o => o.Images)
                .Include(o => o.OrderItems)
                    .ThenInclude(o => o.ProductVariant)
                        .ThenInclude(o => o.Size)
                .Include(o => o.OrderItems)
                    .ThenInclude(o => o.ProductVariant)
                        .ThenInclude(o => o.Color)
                .SingleOrDefaultAsync(o => o.Id == id);

            return View(order);
        }

        public async Task<IActionResult> Confirmed(int? id)
        {
            Order? order = await storeDbContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(o => o.ProductVariant)
                .SingleOrDefaultAsync(o => o.Id == id);
            if(order == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Không tìm thấy đơn hàng!", "Thất bại", NotificationType.Error);
            } else
            {

                foreach(var item in order.OrderItems)
                {
                    item.ProductVariant.InStock -= item.Quantity;
                }

                order.OrderStatus = OrderStatus.CONFIRMED;
                await storeDbContext.SaveChangesAsync();

                NotificationsHelper.AddNotification(HttpContext, "Cập nhật trạng thái đơn hàng thành công!", "Thành công", NotificationType.Success);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delivering(int? id)
        {
            Order? order = await storeDbContext.Orders.FindAsync(id);
            if (order == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Không tìm thấy đơn hàng!", "Thất bại", NotificationType.Error);
            }
            else
            {
                order.OrderStatus = OrderStatus.DELIVERING;
                await storeDbContext.SaveChangesAsync();

                NotificationsHelper.AddNotification(HttpContext, "Cập nhật trạng thái đơn hàng thành công!", "Thành công", NotificationType.Success);
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Rejected(int? id)
        {
            Order? order = await storeDbContext.Orders.FindAsync(id);
            if (order == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Không tìm thấy đơn hàng!", "Thất bại", NotificationType.Error);
            }
            else
            {
                order.OrderStatus = OrderStatus.REJECTED;
                await storeDbContext.SaveChangesAsync();

                NotificationsHelper.AddNotification(HttpContext, "Cập nhật trạng thái đơn hàng thành công!", "Thành công", NotificationType.Success);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
