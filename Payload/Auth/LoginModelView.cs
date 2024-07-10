using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Payload.Auth
{
    public class LoginModelView
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; }
    }
}
