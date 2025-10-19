using ec_project_api.Constants.variables;
using ec_project_api.Interfaces.Shipping;
using ec_project_api.Interfaces.Ships;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.Ships
{
    public class ShipService : BaseService<Ship, byte>, IShipService
    {
        private readonly IShipRepository _shipRepository;

        public ShipService(IShipRepository shipRepository)
            : base(shipRepository)
        {
            _shipRepository = shipRepository;
        }

        public async Task<IEnumerable<Ship>> GetAllAsync(
            int? pageNumber = 1,
            int? pageSize = 10,
            int? statusId = null,
            string? corpName = null,
            string? orderBy = null)
        {
            var options = new QueryOptions<Ship>();

            // Lọc
            options.Filter = s =>
                (s.Status.Name != StatusVariables.Draft) &&
                (!statusId.HasValue || s.StatusId == statusId) &&
                (string.IsNullOrEmpty(corpName) || s.CorpName.Contains(corpName));

            // Sắp xếp
            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy)
                {
                    case "corpname_asc":
                        options.OrderBy = q => q.OrderBy(s => s.CorpName);
                        break;
                    case "corpname_desc":
                        options.OrderBy = q => q.OrderByDescending(s => s.CorpName);
                        break;
                    case "cost_asc":
                        options.OrderBy = q => q.OrderBy(s => s.BaseCost);
                        break;
                    case "cost_desc":
                        options.OrderBy = q => q.OrderByDescending(s => s.BaseCost);
                        break;
                }
            }

            // Phân trang
            options.PageNumber = pageNumber;
            options.PageSize = pageSize;

            // Include
            options.Includes.Add(s => s.Status);

            return await base.GetAllAsync(options);
        }

        public override async Task<Ship?> GetByIdAsync(byte id, QueryOptions<Ship>? options = null)
        {
            options ??= new QueryOptions<Ship>();
            options.Includes.Add(s => s.Status);
            return await base.GetByIdAsync(id, options);
        }

        public async Task<bool> UpdateStatusAsync(byte id, short newStatusId)
        {
            var ship = await _repository.GetByIdAsync(id);
            if (ship == null)
                return false;

            ship.StatusId = newStatusId;
            ship.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(ship);
            return true;
        }
        public async Task<bool> DeleteAsync(Ship entity, short newStatusId)
        {
            var ship = await _repository.GetByIdAsync(entity.ShipId);
            if (ship == null)
            {
                return false;
            }
            if (ship.Status.Name == StatusVariables.Draft)
            {
                await _repository.DeleteAsync(ship);
            }
            else
            {
                await UpdateStatusAsync(ship.ShipId, newStatusId);
            }
            return true;
        }
    }
}
