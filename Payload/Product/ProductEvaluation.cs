using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Payload.Product
{
    public class ProductEvaluation
    {
        [Required(ErrorMessage = "Bạn phải chọn số sao đánh giá")]
        public int Star { get; set; }

        [Required(ErrorMessage = "Nội dung đánh giá là bắt buộc")]
        [StringLength(500, ErrorMessage = "Nội dung đánh giá không được vượt quá 500 ký tự")]
        public string Content { get; set; }
        public List<IFormFile>? Images { get; set; } = new List<IFormFile>();
    }
}
