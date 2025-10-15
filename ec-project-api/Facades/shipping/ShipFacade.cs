using AutoMapper;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.shipping;
using ec_project_api.Dtos.response.shipping;
using ec_project_api.Interfaces.Ships;
using ec_project_api.Interfaces.Shipping;
using ec_project_api.Models;
using ec_project_api.Services;

namespace ec_project_api.Facades.Ships
{
    public class ShipFacade
    {
        private readonly IShipService _shipService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public ShipFacade(IShipService shipService, IStatusService statusService, IMapper mapper)
        {
            _shipService = shipService;
            _statusService = statusService;
            _mapper = mapper;
        }

        // ✅ Lấy danh sách tất cả đơn vị vận chuyển (hỗ trợ filter & paging)
        public async Task<IEnumerable<ShipDto>> GetAllAsync(
            bool isUserAdmin = false,
            int? pageNumber = 1,
            int? pageSize = 10,
            int? statusId = null,
            string? corpName = null,
            string? orderBy = null)
        {
            var ships = await _shipService.GetAllAsync(isUserAdmin, pageNumber, pageSize, statusId, corpName, orderBy);
            return _mapper.Map<IEnumerable<ShipDto>>(ships);
        }

        public async Task<ec_project_api.Dtos.response.pagination.PagedResult<ShipDto>> GetAllPagedAsync(ShipFilter filter)
        {
            var options = new ec_project_api.Repository.Base.QueryOptions<Models.Ship>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            options.Filter = s =>
                ((filter.IsUserAdmin.HasValue && filter.IsUserAdmin.Value) || s.Status.Name != ec_project_api.Constants.variables.StatusVariables.Draft) &&
                (!filter.StatusId.HasValue || s.StatusId == filter.StatusId.Value) &&
                (string.IsNullOrEmpty(filter.CorpName) || s.CorpName.Contains(filter.CorpName));

            if (!string.IsNullOrEmpty(filter.OrderBy))
            {
                switch (filter.OrderBy)
                {
                    case "corpname_asc":
                        options.OrderBy = q => q.OrderBy(s => s.CorpName);
                        break;
                    case "corpname_desc":
                        options.OrderBy = q => q.OrderByDescending(s => s.CorpName);
                        break;
                }
            }

            options.Includes.Add(s => s.Status);

            var paged = await _shipService.GetAllPagedAsync(options);

            var dtoItems = _mapper.Map<IEnumerable<ShipDto>>(paged.Items);
            var pagedDto = new ec_project_api.Dtos.response.pagination.PagedResult<ShipDto>
            {
                Items = dtoItems,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };

            return pagedDto;
        }

        public async Task<ShipDto> GetByIdAsync(byte id)
        {
            var ship = await _shipService.GetByIdAsync(id);
            if (ship == null)
                throw new InvalidOperationException(ShipMessages.ShipNotFound);

            return _mapper.Map<ShipDto>(ship);
        }

        // ✅ Tạo mới đơn vị vận chuyển
        public async Task<bool> CreateAsync(ShipCreateRequest request)
        {
            var draftStatus = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.Ship && s.Name == StatusVariables.Draft
            );

            if (draftStatus == null)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var ship = _mapper.Map<Ship>(request);
            ship.StatusId = draftStatus.StatusId;
            ship.CreatedAt = DateTime.UtcNow;
            ship.UpdatedAt = DateTime.UtcNow;

            var result = await _shipService.CreateAsync(ship);
            if (!result)
                throw new InvalidOperationException(ShipMessages.CreateFailed);

            return result;
        }

        // ✅ Cập nhật đơn vị vận chuyển
        public async Task<bool> UpdateAsync(byte id, ShipUpdateRequest request)
        {
            var existing = await _shipService.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException(ShipMessages.ShipNotFound);

            if (request.StatusId != 0)
            {
                var status = await _statusService.GetByIdAsync(request.StatusId);
                if (status == null)
                    throw new InvalidOperationException(StatusMessages.StatusNotFound);
            }

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            var result = await _shipService.UpdateAsync(existing);
            if (!result)
                throw new InvalidOperationException(ShipMessages.UpdateFailed);

            return result;
        }

        // ✅ Xóa hoặc vô hiệu hóa đơn vị vận chuyển
        public async Task<bool> DeleteAsync(byte id)
        {
            var existing = await _shipService.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException(ShipMessages.ShipNotFound);

            var inactiveStatus = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.Ship && s.Name == StatusVariables.Inactive
            );

            if (inactiveStatus == null)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var result = await _shipService.DeleteAsync(existing, inactiveStatus.StatusId);
            if (!result)
                throw new InvalidOperationException(ShipMessages.DeleteFailed);

            return result;
        }

        // ✅ Cập nhật trạng thái đơn vị vận chuyển
        public async Task<bool> UpdateStatusAsync(byte id, short newStatusId)
        {
            var ship = await _shipService.GetByIdAsync(id);
            if (ship == null)
                throw new InvalidOperationException(ShipMessages.ShipNotFound);

            var status = await _statusService.GetByIdAsync(newStatusId);
            if (status == null)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var result = await _shipService.UpdateStatusAsync(id, newStatusId);
            if (!result)
                throw new InvalidOperationException(ShipMessages.UpdateStatusFailed);

            return result;
        }
    }
}
