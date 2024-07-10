using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double SubTotal { get; set; }

        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
