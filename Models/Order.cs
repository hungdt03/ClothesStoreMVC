using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class Order
    {
        [Key]
        [DisplayName("Mã đơn hàng")]
        public int Id { get; set; }

        [DisplayName("Thời gian đặt")]
        public DateTime CreatedAt { get; set; }

        [DisplayName("Tổng tiền")]
        public double TotalPrice { get; set; }

        [DisplayName("Số lượng")]
        public int Quantity { get; set; }
        public string OrderNote { get; set; }

        [DisplayName("Trạng thái")]
        public string OrderStatus { get; set; }
        public bool IsActive { get; set; } = true;

        public string UserId { get; set; }
        public User User { get; set; }
        public int OrderInfoId { get; set; }
        public OrderInfo OrderInfo { get; set; }

        public int? PaymentId { get; set; }
        public Payment? Payment { get; set; }

        public ICollection<OrderItem> OrderItems { get; set;}
    }
}
