using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Tên thương hiệu")]
        [Required(ErrorMessage = "Chưa nhập tên thương hiệu")]
        public string Name { get; set; }

        [DisplayName("Mô tả thương hiệu")]
        [Required(ErrorMessage = "Chưa nhập mô tả thương hiệu")]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Product>? Products { get; set; }
    }
}
