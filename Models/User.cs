using Microsoft.AspNetCore.Identity;

namespace WebBanQuanAo.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MessagesReceived { get; set; }
        public ICollection<OrderInfo> OrderInfos { get; set; }
        public ICollection<Evaluation> Evaluations { get; set; }
        public Cart Cart { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
