using ec_project_api.Dtos.response;

namespace ec_project_api.Interfaces.location
{
    public interface IProvinceService
    {
        Task<IEnumerable<ProvinceResponseDto>> GetAllProvincesAsync();
    }
}

