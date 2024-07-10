using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class ProductVariant
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Chưa nhập số lượng tồn kho")]
        [Display(Name = "Tồn kho")]
        public int InStock { get; set; }

        [Required(ErrorMessage = "Chưa chọn kích cỡ sản phẩm.")]
        [Display(Name = "Kích cỡ")]
        public int SizeId { get; set; }

        [Required(ErrorMessage = "Chưa chọn màu sắc sản phẩm.")]
        [Display(Name = "Màu sắc")]
        public int ColorId { get; set; }

        [Required(ErrorMessage = "Chưa chọn một sản phẩm.")]
        [Display(Name = "Sản phẩm")]
        public int ProductId { get; set; }
        public Size? Size {  get; set; }
        public Color? Color { get; set; }
        public Product? Product { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
        public ICollection<ProductVariantImage>? Images { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
