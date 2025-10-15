using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Interfaces.Ships
{
    public interface IShipService : IBaseService<Ship, byte>
    {
        Task<IEnumerable<Ship>> GetAllAsync(
            bool isUserAdmin = false,
            int? pageNumber = 1,
            int? pageSize = 10,
            int? statusId = null,
            string? corpName = null,
            string? orderBy = null);

        Task<bool> UpdateStatusAsync(byte id, short newStatusId);
        Task<bool> DeleteAsync(Ship entity, short newStatusId);
    }
}
