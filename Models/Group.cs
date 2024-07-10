using System.ComponentModel.DataAnnotations;

namespace WebBanQuanAo.Models
{
    public class Group
    {
        [Key]
        public string GroupName { get; set; }
        public ICollection<Connection> Connections { get; set; }
    }
}
