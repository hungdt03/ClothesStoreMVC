using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Payload.Product.Variant
{
    public class EditVariantModelView
    {
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

        //[Required(ErrorMessage = "Chưa chọn hình ảnh.")]
        //[Display(Name = "Hình ảnh")]
        public List<IFormFile>? Images { get; set; }
    }
}
