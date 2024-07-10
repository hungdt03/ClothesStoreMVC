namespace WebBanQuanAo.Payload.Cart
{
    public class UpdateCartPayload
    {
        public List<CartItemPayload> CartItems { get; set; }
    }

    public class CartItemPayload
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }

}
