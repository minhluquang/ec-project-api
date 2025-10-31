using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.wards
{
    public interface IWardService : IBaseService<Ward, int>
    {
        Task<IEnumerable<Ward>> GetWardsByProvinceIdAsync(int provinceId);
    }
}