using ec_project_api.Models;
using ec_project_api.Models.location;

namespace ec_project_api.Interfaces.location
{
    public interface IProvinceRepository
    {
        Task<IEnumerable<Province>> GetAllProvincesAsync();
        Task<Province?> GetProvinceByIdAsync(int id);
    }
}

