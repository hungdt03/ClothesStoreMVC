using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class Size
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên kích cỡ không được để trống")]
        [DisplayName("Tên kích cỡ")]
        public string ESize { get; set; }

        [Required(ErrorMessage = "Chiều cao tối thiểu không được để trống")]
        [DisplayName("Chiều cao tối thiểu")]
        public double MinHeight { get; set; }

        [Required(ErrorMessage = "Chiều cao tối đa không được để trống")]
        [DisplayName("Chiều cao tối đa")]
        public double MaxHeight { get; set; }

        [Required(ErrorMessage = "Cân nặng tối thiểu không được để trống")]
        [DisplayName("Cân nặng tối thiểu")]
        public double MinWeight { get; set; }

        [Required(ErrorMessage = "Cân nặng tối đa không được để trống")]
        [DisplayName("Cân nặng tối đa")]
        public double MaxWeight { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<ProductVariant>? ProductVariants { get; set; }
    }
}
