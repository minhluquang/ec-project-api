using ec_project_api.Dtos.response.pagination;
namespace ec_project_api.Dtos.response.products;

public class PagedResultWithCategory<T> : PagedResult<T>
{
    public string? CategoryName { get; set; }
}