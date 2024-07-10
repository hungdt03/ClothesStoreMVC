using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string SenderId { get; set; }
        public User Sender { get; set; }
        public string RecipientId { get; set; }
        public User Recipient { get; set; }
        public string Content { get; set; }
        public DateTime SendAt { get; set; } = DateTime.Now;
    }
}
