using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanQuanAo.Models
{
    public class OrderInfo
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsDefault { get; set; } = true;
        public string? UserId { get; set; }
        public User? User { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
