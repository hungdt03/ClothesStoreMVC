
using System.ComponentModel;

namespace WebBanQuanAo.Payload.Product
{
    public class ProductViewModel
    {
        [DisplayName("Tên sản phẩm")]
        public string Name { get; set; }

        [DisplayName("Mô tả sản phẩm")]
        public string Description { get; set; }

        [DisplayName("Giá bán")]
        public double Price { get; set; }

        [DisplayName("Danh mục")]
        public int? CategoryId { get; set; }

        [DisplayName("Thương hiệu")]
        public int? BrandId { get; set; }
        [DisplayName("Hình ảnh đại diện")]
        public IFormFile? Thumbnail { get; set; }
        [DisplayName("Hình ảnh phóng to")]
        public IFormFile? ZoomImage { get; set; }

        public ICollection<int>? ColorIds { get; set; }
        public ICollection<int>? SizeIds { get; set; }

        [DisplayName("Hình ảnh")]
        public List<IFormFile>? Images { get; set; }
    }
}
