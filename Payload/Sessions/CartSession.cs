using Microsoft.AspNetCore.Routing.Constraints;

namespace WebBanQuanAo.Payload.Sessions
{
    public class CartSession
    {
        public List<CartItemSession> Items { get; set; }
        public double TotalPrice { get; set; }
        
    }
}
