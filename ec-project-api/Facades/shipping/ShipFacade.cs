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

        // ✅ Lấy danh sách tất cả đơn vị vận chuyển
        public async Task<IEnumerable<ShipDto>> GetAllAsync()
        {
            var ships = await _shipService.GetAllAsync();
            return _mapper.Map<IEnumerable<ShipDto>>(ships);
        }

        // ✅ Lấy đơn vị vận chuyển theo ID
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
