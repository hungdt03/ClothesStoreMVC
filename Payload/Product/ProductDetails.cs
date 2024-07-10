
namespace WebBanQuanAo.Payload.Product
{
    public class ProductDetails
    {
        public Models.Product Product { get; set; }
        public List<Models.Color> Colors { get; set; }
        public List<Models.Size> Sizes { get; set; }
        public List<Models.Evaluation> Reviews { get; set; }

    }
}
