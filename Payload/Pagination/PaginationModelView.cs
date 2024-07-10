namespace WebBanQuanAo.Payload.Pagination
{
    public class PaginationModelView<T> : GeneralPaginationModelView
    {
        public List<T> TotalRows { get; set; }
        public List<T> Items { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string Status { get; set; }

        
    }

    public interface GeneralPaginationModelView
    {

    }
}
