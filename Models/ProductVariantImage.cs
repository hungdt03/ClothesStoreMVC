using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class ProductVariantImage
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; }

        public int? ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
