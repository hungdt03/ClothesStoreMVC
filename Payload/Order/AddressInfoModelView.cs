using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Payload.Order
{
    public class AddressInfoModelView
    {
        [Required(ErrorMessage = "Họ không được để trống")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Tên không được để trống")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tỉnh thành")]
        public string Province { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quận huyện")]
        public string District { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phường xã")]
        public string Commune { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số nhà")]
        public string HomeNumber { get; set; }
    }
}
