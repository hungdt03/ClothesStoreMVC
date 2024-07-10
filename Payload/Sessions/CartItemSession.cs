namespace WebBanQuanAo.Payload.Sessions
{
    public class CartItemSession
    {
        public int ProductVariantId { get; set; }
        public double Price { get; set; }
        public double SubTotal { get; set; }
        public int Quantity { get; set; }
    }
}
