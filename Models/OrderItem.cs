using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double SubTotal { get; set; }
        
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
