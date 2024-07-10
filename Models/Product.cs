using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Tên sản phẩm")]
        public string Name { get; set; }

        [DisplayName("Mô tả sản phẩm")]
        public string Description { get; set; }

        [DisplayName("Giá bán")]
        public double Price { get; set; }
        public string Thumbnail {  get; set; }
        public string ZoomImage { get; set; }

        public bool IsActive { get; set; } = true;

        [DisplayName("Danh mục")]
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        [DisplayName("Thương hiệu")]
        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }

        public ICollection<Image>? Images { get; set; }
        public ICollection<ProductVariant> ProductVariants { get; set; }
        public ICollection<Evaluation> Evaluations { get; set; }

    }
}
