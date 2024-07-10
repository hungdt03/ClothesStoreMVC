using Microsoft.AspNetCore.SignalR;
using WebBanQuanAo.SignalR.Payload;

namespace WebBanQuanAo.SignalR.Hubs
{
    public class OrderNotificationHub : Hub
    {
        public async Task SendOrderNotification(OrderNotification notification)
        {
            await Clients.All.SendAsync("ReceiveOrderNotification", notification);
        }
    }
}
