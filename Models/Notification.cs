using System.ComponentModel.DataAnnotations;
using WebBanQuanAo.Constants;
using WebBanQuanAo.Helpers;

namespace WebBanQuanAo.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public User? User { get; set; }
        public string? UserId { get; set; }
        public string ToTypeUser { get; set; }
        public bool HaveRead { get; set; }
        public string AccessUrl { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
