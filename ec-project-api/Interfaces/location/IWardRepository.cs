using ec_project_api.Models;
using ec_project_api.Models.location;

namespace ec_project_api.Interfaces.location
{
    public interface IWardRepository
    {
        Task<IEnumerable<Ward>> GetWardsByProvinceIdAsync(int provinceId);
        Task<Ward?> GetWardByIdAsync(int id);
    }
}
