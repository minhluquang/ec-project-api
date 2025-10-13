using ec_project_api.Dtos.response;

namespace ec_project_api.Interfaces.location
{
    public interface IWardService
    {
        Task<IEnumerable<WardResponseDto>> GetWardsByProvinceIdAsync(int provinceId);
    }
}
