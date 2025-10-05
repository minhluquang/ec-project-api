using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.sizes
{
    public class SizeService : BaseService<Size, byte>, ISizeService
    {
        public SizeService(ISizeRepository repository) : base(repository)
        {
        }
    }
}