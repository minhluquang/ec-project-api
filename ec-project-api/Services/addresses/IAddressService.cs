using ec_project_api.DTOs.response.addresses;
using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.addresses
{
    public interface IAddressService : IBaseService<Address, int>
    {
         Task<IEnumerable<Address>> GetByUserIdAsync(int userId);
    }
}