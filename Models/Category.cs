using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        [DisplayName("Tên danh mục")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mô tả không được để trống")]
        [DisplayName("Mô tả danh mục")]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
        [Display(Name = "Danh mục cha")]
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { set; get; }
        public ICollection<Category>? CategoryChildren { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
