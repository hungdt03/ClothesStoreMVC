using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class Color
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Tên màu sắc")]
        [Required(ErrorMessage = "Chưa nhập tên màu sắc")]
        public string Name { get; set; }
        public string HexCode { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<ProductVariant>? ProductVariants { get; set; }
    }
}
