using WebBanQuanAo.Models;

namespace WebBanQuanAo.Payload
{
    public class ShopModelView
    {
        public Category Category { get; set; }
        public List<Models.Product> Products { get; set; }
        public List<Brand> Brands { get; set; }
        public List<Category> Categories { get; set; }
        public List<Color> Colors { get; set; }
        public List<Size> Sizes { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }

        public List<int> CheckedBrandIds { get; set; } = new List<int>();
        public List<int> CheckedSizeIds { get; set; } = new List<int>();
        public List<int> CheckedColorIds { get; set; } = new List<int>();
        public double SelectMinPrice { get; set; } = 0;
        public double SelectMaxPrice { get; set; } = 0;

        public List<Models.Product> TotalRows { get; set; }
        public int TotalPages { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
