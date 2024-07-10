using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PayPal.Api;
using System.Security.Claims;
using WebBanQuanAo.Clients;
using WebBanQuanAo.Constants;
using WebBanQuanAo.Data;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Controllers
{
    public class PaymentController : Controller
    {
        private readonly StoreDbContext _storeContext;
        private readonly PaypalClient _paypalClient;

        public PaymentController(StoreDbContext storeContext, PaypalClient paypalClient)
        {
            _storeContext = storeContext;
            _paypalClient = paypalClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VnpayConfirm(
            [FromQuery] decimal vnp_Amount,
            [FromQuery] string vnp_BankCode,
            [FromQuery] string vnp_BankTranNo,
            [FromQuery] string vnp_CardType,
            [FromQuery] string vnp_OrderInfo,
            [FromQuery] DateTime vnp_PayDate,
            [FromQuery] string vnp_ResponseCode,
            [FromQuery] string vnp_TmnCode,
            [FromQuery] string vnp_TransactionNo,
            [FromQuery] string vnp_TransactionStatus,
            [FromQuery] string vnp_TxnRef
        )
        {

            if (vnp_ResponseCode.Equals("00") && vnp_TransactionStatus.Equals("00"))
            {
                var userId = User?.FindFirst(ClaimTypes.Sid)?.Value ?? "";
                Cart cart = await GetCart();

                var order = await CreateOrder(cart, userId);
                var orderNote = HttpContext.Session.GetString("OrderNote");
                order.OrderNote = orderNote;

                Models.Payment orderPayment = new Models.Payment();
                orderPayment.Status = true;
                orderPayment.PaymentCode = vnp_BankTranNo;
                orderPayment.PaymentType = PaymentType.VNPAY;
                order.Payment = orderPayment;
                orderPayment.PaymentDate = DateTime.Now;

                EntityEntry<Models.Order> savedOrder = await _storeContext.Orders.AddAsync(order);

                // Clear cart
                cart.Quantity = 0;
                cart.TotalPrice = 0;

                _storeContext.CartItems.RemoveRange(cart.CartItems);
                await _storeContext.SaveChangesAsync();

                return View("SuccessfulPayment");

            }

            return View("FailedPayment");

        }

        public async Task<IActionResult> Capture(string orderId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderId);

                var reference = response.purchase_units[0].reference_id;

                var userId = User?.FindFirst(ClaimTypes.Sid)?.Value ?? "";
                Cart cart = await GetCart();

                var order = await CreateOrder(cart, userId);

                Models.Payment orderPayment = new Models.Payment();
                orderPayment.Status = true;
                orderPayment.PaymentCode = reference;
                orderPayment.PaymentType = PaymentType.VNPAY;
                orderPayment.PaymentDate = DateTime.Now;
                order.Payment = orderPayment;

                EntityEntry<Models.Order> savedOrder = await _storeContext.Orders.AddAsync(order);

                // Clear cart
                cart.Quantity = 0;
                cart.TotalPrice = 0;

                _storeContext.CartItems.RemoveRange(cart.CartItems);
                await _storeContext.SaveChangesAsync();

                return View("SuccessfulPayment");
            }
            catch 
            {
                return View("FailedPayment");
            }

            
        }

        private async Task<OrderInfo> GetOrderInfo(string userId)
        {
            string orderInfoId = HttpContext.Session.GetString("AddressInfoId");
            if (orderInfoId == null)
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

            

            var oderInfo = await GetOrderInfo(userId);
            order.OrderInfo = oderInfo;
            order.OrderNote = "";
            order.Quantity = cart.Quantity;
            order.TotalPrice = cart.TotalPrice;
            order.OrderStatus = OrderStatus.PENDING;
            
            order.UserId = userId;
            order.CreatedAt = DateTime.Now;

            return order;
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

    }

  
}
