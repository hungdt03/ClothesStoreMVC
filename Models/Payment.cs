
using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public string PaymentType { get; set; }
        public string PaymentCode { get; set; }
        public bool Status { get; set; }
        public DateTime PaymentDate { get; set; }
        public Order Order { get; set; }
    }
}
