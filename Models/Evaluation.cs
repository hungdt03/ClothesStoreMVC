using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class Evaluation
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public int Stars { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public DateTime DateCreated {  get; set; }

        public ICollection<ImageEvaluation> Images { get; set; }
        public int? Favorites {  get; set; }

    }
}
