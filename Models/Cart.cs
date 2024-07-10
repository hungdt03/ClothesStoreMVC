using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public ICollection<CartItem> CartItems {  get; set; } 
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
