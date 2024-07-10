namespace WebBanQuanAo.SignalR.Payload
{
    public class OrderNotification
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalAmount { get; set; }
    }
}
