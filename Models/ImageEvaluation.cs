using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class ImageEvaluation
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; }
        public int EvaluationId { get; set; }
        public Evaluation Evaluation { get; set; }

    }
}
