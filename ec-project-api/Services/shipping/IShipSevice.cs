using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Interfaces.Ships
{
    public interface IShipService : IBaseService<Ship, short>
    {
        Task<IEnumerable<Ship>> GetAllAsync(
            int? pageNumber = 1,
            int? pageSize = 10,
            int? statusId = null,
            string? corpName = null,
            string? orderBy = null);

        Task<bool> SetActiveStatusAsync(Ship ship, short activeStatusId, short inactiveStatusId);
    }
}
