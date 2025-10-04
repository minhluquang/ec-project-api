namespace ec_project_api.Dtos.response
{
   public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public PagedResponse(IEnumerable<T> items, int totalItems, int page, int pageSize)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            TotalItems = totalItems;
            Page = page;
            PageSize = pageSize;
        }
    }
}
