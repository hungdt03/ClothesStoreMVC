namespace WebBanQuanAo.Payload.Product
{
    public class ProductViewHome
    {
        public List<Models.Product> Products { get; set; }
        public List<Models.Product> FavoriteProducts { get; set; }
        public List<Models.Product> BestSellerProducts { get; set; }

    }
}
