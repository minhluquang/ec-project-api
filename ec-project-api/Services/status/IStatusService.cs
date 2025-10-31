using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services
{
    public interface IStatusService : IBaseService<Status, short>
    { Task<Status?> GetByNameAndEntityTypeAsync(string name, string entityType); }
}