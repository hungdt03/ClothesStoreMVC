using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Payload.Auth
{
    public class SignUpModelView
    {
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [Display(Name = "Fullname")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Username không được để trống")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        //[DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
