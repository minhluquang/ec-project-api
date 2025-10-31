using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Interfaces.Shipping;
using ec_project_api.Interfaces.Ships;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.Ships
{
    public class ShipService : BaseService<Ship, short>, IShipService
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
            
            options.Includes.Add(s => s.Status);

            // Lọc
            options.Filter = s =>
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
            } else
            {
                options.OrderBy = q => q
                    .OrderByDescending(s => s.Status.Name == StatusVariables.Active)
                    .ThenByDescending(s => s.CreatedAt);
            }

            // Phân trang
            options.PageNumber = pageNumber;
            options.PageSize = pageSize;

            return await base.GetAllAsync(options);
        }

        public override async Task<Ship?> GetByIdAsync(short id, QueryOptions<Ship>? options = null)
        {
            options ??= new QueryOptions<Ship>();
            options.Includes.Add(s => s.Status);
            return await base.GetByIdAsync(id, options);
        }

        public async Task<bool> SetActiveStatusAsync(Ship ship, short activeStatusId, short inactiveStatusId)
        {
            var options = new QueryOptions<Ship>
            {
                Filter = s => s.StatusId == activeStatusId && s.ShipId != ship.ShipId
            };
            var otherActiveShips = await _repository.GetAllAsync(options);

            ship.StatusId = activeStatusId;
            ship.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(ship);

            // Update other ships
            foreach (var s in otherActiveShips)
            {
                s.StatusId = inactiveStatusId;
                s.UpdatedAt = DateTime.UtcNow;
                await _repository.UpdateAsync(s);
            }

            await _repository.SaveChangesAsync();
            return true;
        }
        
        // csharp
        public override async Task<bool> DeleteByIdAsync(short id)
        {
            var options = new QueryOptions<Ship>();
            options.Includes.Add(s => s.Orders);
            options.Includes.Add(s => s.Status);

            var ship = await _repository.GetByIdAsync(id, options);
            if (ship == null)
                throw new KeyNotFoundException(ShipMessages.ShipNotFound);

            if (ship.Orders != null && ship.Orders.Count > 0)
                throw new InvalidOperationException(ShipMessages.ShipInUse);

            if (ship.Status.Name == StatusVariables.Active)
                throw new InvalidOperationException(ShipMessages.ShipActive);

            await _repository.DeleteAsync(ship);
            return await _repository.SaveChangesAsync() > 0;
        }
    }
}
