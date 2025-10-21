using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services {
    public interface ISizeService : IBaseService<Size, byte> {
        Task<IEnumerable<Size>> GetSizeOptionsAsync(QueryOptions<Size>? options = null);
    }
}