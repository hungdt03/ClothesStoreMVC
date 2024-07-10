namespace WebBanQuanAo.Payload
{
    public class FilterPayload
    {
        public int CategoryId { get; set; }
        public List<int> SizeIds { get; set; } = new List<int>();
        public double MinPrice {  get; set; } 
        public double MaxPrice { get; set; }
        public List<int> ColorIds { get; set; } = new List<int>();
        public List<int> BrandIds { get; set; } = new List<int>();
       
      
    }
}
