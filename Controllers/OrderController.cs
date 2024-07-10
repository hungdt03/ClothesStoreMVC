using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;
using System.Text;
using WebBanQuanAo.Clients;
using WebBanQuanAo.Constants;
using WebBanQuanAo.Data;
using WebBanQuanAo.Helper;
using WebBanQuanAo.Helpers;
using WebBanQuanAo.Models;
using WebBanQuanAo.Payload.Order;
using WebBanQuanAo.Payload.Pagination;
using WebBanQuanAo.SignalR.Hubs;


namespace WebBanQuanAo.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const double UsdPrice = 23500;
        private readonly StoreDbContext _storeContext;
        private readonly ILogger<OrderController> _logger;
        private readonly IHubContext<OrderNotificationHub> _hubContext;
        private readonly IConfiguration _configuration;
        private readonly VnpayHelper vnpay;
        private readonly VnPayUtils vnPayUtils;
        private readonly PaypalClient _paypalClient;


        public OrderController(StoreDbContext storeContext, ILogger<OrderController> logger, IHubContext<OrderNotificationHub> hubContext, IConfiguration configuration, VnpayHelper vnpay, VnPayUtils vnPayUtils, PaypalClient paypalClient)
        {
            _storeContext = storeContext;
            _logger = logger;
            _hubContext = hubContext;
            _configuration = configuration;
            this.vnpay = vnpay;
            this.vnPayUtils = vnPayUtils;
            this._paypalClient = paypalClient;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User?.FindFirst(ClaimTypes.Sid)?.Value ?? "";
            Cart? cart = await GetCart();

            List<OrderInfo> orderInfos = await _storeContext.OrderInfos
                .Where(o => o.UserId.Equals(userId)).ToListAsync();

            ViewBag.ClientId = _paypalClient.ClientId;
            ViewBag.Cart = cart;
            ViewBag.OrderInfos = orderInfos;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrderPayPal(CancellationToken cancellationToken)
        {
            try
            {
                var userId = User?.FindFirst(ClaimTypes.Sid)?.Value ?? "";
                Cart? cart = await _storeContext.Carts
                    .SingleOrDefaultAsync(c => c.UserId.Equals(userId));
                // set the transaction price and currency
                var price = Math.Round(cart.TotalPrice/UsdPrice, 2).ToString();
                var currency = "USD";

                // "reference" is the transaction key
                var reference = "DH" + DateTime.Now.Ticks.ToString();

                var response = await _paypalClient.CreateOrder(price, currency, reference);

                return Ok(response);
            }
            catch (Exception e)
            {
                var error = new
                {
                    e.GetBaseException().Message
                };

                return BadRequest(error);
            }
        }

        private async Task<OrderInfo> GetOrderInfo(string userId)
        {
            string orderInfoId = HttpContext.Session.GetString("AddressInfoId");
            if(orderInfoId == null)
            {
                OrderInfo? defaultOrderInfo = await _storeContext.OrderInfos
                    .FirstOrDefaultAsync(o => o.UserId.Equals(userId) && o.IsDefault);

                return defaultOrderInfo!;
            }

            OrderInfo? orderInfo = await _storeContext.OrderInfos
                    .FirstOrDefaultAsync(o => o.Id == int.Parse(orderInfoId));

            return orderInfo!;
        }

        private async Task<Models.Order> CreateOrder(Cart cart, string userId)
        {
            Models.Order order = new Models.Order();
            order.OrderItems = new List<OrderItem>();

            foreach (var item in cart.CartItems)
            {
                OrderItem orderItem = new OrderItem();
                orderItem.SubTotal = item.SubTotal;
                orderItem.Quantity = item.Quantity;
                orderItem.Price = item.Price;
                orderItem.ProductVariantId = item.ProductVariantId;

                order.OrderItems.Add(orderItem);
            }

            Models.Payment orderPayment = new Models.Payment();
            orderPayment.Status = false;
            orderPayment.PaymentCode = userId;
            orderPayment.PaymentType = PaymentType.CASH;

            var oderInfo = await GetOrderInfo(userId);
            order.OrderInfo = oderInfo;
            order.OrderNote = "";
            order.Quantity = cart.Quantity;
            order.TotalPrice = cart.TotalPrice;
            order.OrderStatus = OrderStatus.PENDING;
            order.Payment = orderPayment;
            order.UserId = userId;
            order.CreatedAt = DateTime.Now;

            return order;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrderCash(OrderInfoModelView model)
        {
            var userId = User?.FindFirst(ClaimTypes.Sid)?.Value ?? "";
            Cart cart = await GetCart();

            var order = await CreateOrder(cart, userId);

            EntityEntry<Models.Order> savedOrder = await _storeContext.Orders.AddAsync(order);

            // Clear cart
            cart.Quantity = 0;
            cart.TotalPrice = 0;

            _storeContext.CartItems.RemoveRange(cart.CartItems);
            await _storeContext.SaveChangesAsync();

            NotificationsHelper.AddNotification(HttpContext, "Đặt hàng thành công!", "Thành công", NotificationType.Success);

            // Create notification
            var fullName = order.OrderInfo.FirstName + " " + order.OrderInfo.LastName;
            Models.Notification notification = new Models.Notification()
            {
                Title = "Đơn đặt hàng mới",
                Message = $"{fullName} vừa đặt đơn hàng mới",
                AccessUrl = $"http://localhost:5029/Admin/Orders/{savedOrder.Entity.Id}",
                CreatedAt = DateTime.Now,
                HaveRead = false,
                ToTypeUser = ToTypeUser.TO_ADMIN,
            };

            await _storeContext.Notifications.AddAsync(notification);
            await _storeContext.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveOrderNotification", notification);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrderVnpay(OrderInfoModelView model)
        {
            HttpContext.Session.SetString("OrderNote", model.OrderNote);
            var urlPayment = await getPaymentVnpayUrl();
            return Ok(new
            {
                Url = urlPayment
            });
        }
        
        private async Task<Cart> GetCart()
        {
            var userId = User?.FindFirst(ClaimTypes.Sid)?.Value ?? "";
            Cart? cart = await _storeContext.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant.Product)
                        .ThenInclude(product => product.Images)
                .Include(c => c.CartItems)
                    .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant.Color)
                .Include(c => c.CartItems)
                    .ThenInclude(item => item.ProductVariant)
                    .ThenInclude(variant => variant.Size)
                .SingleOrDefaultAsync(c => c.UserId.Equals(userId));

            return cart;
        }
       

        public IActionResult Success()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SetAddressInfoOrder([FromBody] SetAdrressInfoPayload payload)
        {
            try
            {
                HttpContext.Session.SetString("AddressInfoId", payload.OrderInfoId.ToString());
                return Ok();
            } catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress(AddressInfoModelView modelView)
        {

            if(!ModelState.IsValid)
            {
                return View("Index", modelView);
            }
            var orders = await _storeContext.OrderInfos.ToListAsync();
            foreach(var item in orders)
            {
                item.IsDefault = false;
            }

            var userId = User?.FindFirst(ClaimTypes.Sid)?.Value ?? "";
            OrderInfo orderInfo = new OrderInfo()
            {
                FirstName = modelView.FirstName,
                LastName = modelView.LastName,
                Address = modelView.Address,
                Email = modelView.Email,
                PhoneNumber = modelView.PhoneNumber,
                UserId = userId
            };

            await _storeContext.AddAsync(orderInfo);
            await _storeContext.SaveChangesAsync();

            return RedirectToAction("Index", "Order");

        }
       
        private async Task<string> getPaymentVnpayUrl()
        {
           
            var urlPayment = "";
            //Get Config Info
            string vnp_Returnurl = _configuration["VnpaySettings:vnp_Returnurl"];
            string vnp_Url = _configuration["VnpaySettings:vnp_Url"];
            string vnp_TmnCode = _configuration["VnpaySettings:vnp_TmnCode"];
            string vnp_HashSecret = _configuration["VnpaySettings:vnp_HashSecret"];

            //Build URL for VNPAY
            var reference = "DH" + DateTime.Now.Ticks.ToString();
            var cart = await GetCart();
            var total = cart.TotalPrice * 100;
            vnpay.AddRequestData("vnp_Version", VnpayHelper.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", total.ToString());
   

            vnpay.AddRequestData("vnp_BankCode", "VNBANK");

            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", vnPayUtils.GetIpAddress());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán cho hóa đơn có mã số: " + reference);
            vnpay.AddRequestData("vnp_OrderType", "other");

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", reference);


            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return urlPayment;
        }

        private static string GenerateCode(int orderId)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder result = new StringBuilder(10);
            result.Append(orderId.ToString());
            result.Append("-");
            Random random = new Random();

            for (int i = 0; i < 10; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }


            return result.ToString();
        }

        [HttpGet]
        public async Task<IActionResult> OrderHistory(int pageSize = 5, int pageIndex = 1, string status = "")
        {
            string statusOrderHistory = "";
            if(!string.IsNullOrEmpty(status))
            {
                if(status.Equals("Pending"))
                {
                    statusOrderHistory = OrderStatus.PENDING;
                } else if(status.Equals("Confirmed"))
                {
                    statusOrderHistory = OrderStatus.CONFIRMED;
                } else if(status.Equals("Delivering"))
                {
                    statusOrderHistory = OrderStatus.DELIVERING;
                } else if(status.Equals("Completed"))
                {
                    statusOrderHistory = OrderStatus.COMPLETED;
                } else
                {
                    status = "";
                }
            }
            var userId = User?.FindFirst(ClaimTypes.Sid)?.Value ?? "";
            IQueryable<Models.Order> queryable = _storeContext.Orders
                .Where(o => o.UserId == userId && o.OrderStatus.Contains(statusOrderHistory))
                .Include(o => o.OrderInfo)
                .Include(o => o.OrderItems)
                    .ThenInclude(o => o.ProductVariant)
                        .ThenInclude(o => o.Size)
                .Include(o => o.OrderItems)
                    .ThenInclude(o => o.ProductVariant)
                        .ThenInclude(o => o.Color)
                .Include(o => o.OrderItems)
                    .ThenInclude(o => o.ProductVariant)
                        .ThenInclude(o => o.Images)
                .Include(o => o.OrderItems)
                    .ThenInclude(o => o.ProductVariant)
                        .ThenInclude(o => o.Product)
                .Include(o => o.Payment);

            List<Models.Order> items = await queryable
                .OrderByDescending(s => s.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            PaginationModelView<Models.Order> modelView = new PaginationModelView<Models.Order>()
            {
                TotalRows = await queryable.ToListAsync(),
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                Status = string.IsNullOrEmpty(status) ? "All" : status,
            
            };

            return View(modelView);
        }

        public async Task<IActionResult> Received(int id)
        {
            Models.Order? order = await _storeContext.Orders
                .Include(o => o.Payment)
                .SingleOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Không tìm thấy đơn hàng!", "Thất bại", NotificationType.Error);
            }
            else
            {
                order.OrderStatus = OrderStatus.COMPLETED;
                order.Payment.Status = true;
                order.Payment.PaymentDate = DateTime.Now;
                await _storeContext.SaveChangesAsync();

                NotificationsHelper.AddNotification(HttpContext, "Cập nhật trạng thái đơn hàng thành công!", "Thành công", NotificationType.Success);
            }

            return RedirectToAction(nameof(OrderHistory));
        }

        public async Task<IActionResult> Cancel(int id)
        {
            Models.Order? order = await _storeContext.Orders.FindAsync(id);
            if (order == null)
            {
                NotificationsHelper.AddNotification(HttpContext, "Không tìm thấy đơn hàng!", "Thất bại", NotificationType.Error);
            }
            else
            {
                order.OrderStatus = OrderStatus.CANCELLED;
                await _storeContext.SaveChangesAsync();

                NotificationsHelper.AddNotification(HttpContext, "Cập nhật trạng thái đơn hàng thành công!", "Thành công", NotificationType.Success);
            }

            return RedirectToAction(nameof(OrderHistory));
        }
    }

}
