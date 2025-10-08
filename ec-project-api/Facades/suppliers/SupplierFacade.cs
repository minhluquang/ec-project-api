using AutoMapper;
using ec_project_api.Dtos.request.suppliers;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.suppliers;
using ec_project_api.Interfaces.Suppliers;
using ec_project_api.Models;
using ec_project_api.Constants.variables;
using ec_project_api.Constants.Messages;
using ec_project_api.Services;

namespace ec_project_api.Facades.Suppliers
{
    public class SupplierFacade
    {
        private readonly ISupplierService _supplierService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public SupplierFacade(ISupplierService supplierService, IStatusService statusService, IMapper mapper)
        {
            _supplierService = supplierService;
            _statusService = statusService;
            _mapper = mapper;
        }

        // Lấy tất cả nhà cung cấp
        public async Task<ResponseData<IEnumerable<SupplierDto>>> GetAllAsync()
        {
            var suppliers = await _supplierService.GetAllAsync();
            var result = _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
            return ResponseData<IEnumerable<SupplierDto>>.Success(StatusCodes.Status200OK, result);
        }

        // Lấy theo ID
        public async Task<ResponseData<SupplierDto>> GetByIdAsync(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
                return ResponseData<SupplierDto>.Error(StatusCodes.Status404NotFound, "Không tìm thấy nhà cung cấp.");

            var result = _mapper.Map<SupplierDto>(supplier);
            return ResponseData<SupplierDto>.Success(StatusCodes.Status200OK, result);
        }

        // Tạo mới Supplier
        public async Task<ResponseData<SupplierDto>> CreateAsync(SupplierCreateRequest request)
        {
            // ✅ Lấy status mặc định (ví dụ: Pending)
            var pendingStatus = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.Supplier && s.Name == StatusVariables.Pending
            );

            if (pendingStatus == null)
                return ResponseData<SupplierDto>.Error(StatusCodes.Status400BadRequest, StatusMessages.StatusNotFound);

            var supplier = _mapper.Map<Supplier>(request);
            supplier.StatusId = pendingStatus.StatusId;

            var success = await _supplierService.CreateAsync(supplier);
            if (!success)
                return ResponseData<SupplierDto>.Error(StatusCodes.Status400BadRequest, "Không thể tạo nhà cung cấp.");

            var dto = _mapper.Map<SupplierDto>(supplier);
            return ResponseData<SupplierDto>.Success(StatusCodes.Status201Created, dto, "Tạo nhà cung cấp thành công.");
        }

        // Cập nhật Supplier
        public async Task<ResponseData<bool>> UpdateAsync(int id, SupplierUpdateRequest request)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
                return ResponseData<bool>.Error(StatusCodes.Status404NotFound, "Không tìm thấy nhà cung cấp.");

            // Nếu có cập nhật trạng thái thì kiểm tra status có hợp lệ không
            if (request.StatusId != 0)
            {
                var status = await _statusService.GetByIdAsync(request.StatusId);
                if (status == null)
                    return ResponseData<bool>.Error(StatusCodes.Status400BadRequest, "Trạng thái không hợp lệ.");
            }

            _mapper.Map(request, supplier);
            supplier.UpdatedAt = DateTime.UtcNow;

            var success = await _supplierService.UpdateAsync(supplier);
            if (!success)
                return ResponseData<bool>.Error(StatusCodes.Status400BadRequest, "Không thể cập nhật nhà cung cấp.");

            return ResponseData<bool>.Success(StatusCodes.Status200OK, true, "Cập nhật nhà cung cấp thành công.");
        }

        // Xóa Supplier
        public async Task<ResponseData<bool>> DeleteAsync(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
                return ResponseData<bool>.Error(StatusCodes.Status404NotFound, "Không tìm thấy nhà cung cấp.");

            var success = await _supplierService.DeleteAsync(supplier);
            if (!success)
                return ResponseData<bool>.Error(StatusCodes.Status400BadRequest, "Không thể xóa nhà cung cấp.");

            return ResponseData<bool>.Success(StatusCodes.Status200OK, true, "Xóa nhà cung cấp thành công.");
        }

        public async Task<ResponseData<bool>> UpdateStatusAsync(int id, int newStatusId)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
                return ResponseData<bool>.Error(StatusCodes.Status404NotFound, "Không tìm thấy nhà cung cấp.");

            var status = await _statusService.GetByIdAsync(newStatusId);
            if (status == null)
                return ResponseData<bool>.Error(StatusCodes.Status400BadRequest, "Trạng thái không hợp lệ.");

            var result = await _supplierService.UpdateStatusAsync(id, newStatusId);
            if (!result)
                return ResponseData<bool>.Error(StatusCodes.Status400BadRequest, "Không thể cập nhật trạng thái.");

            return ResponseData<bool>.Success(StatusCodes.Status200OK, true, "Cập nhật trạng thái thành công.");
        }
    }
}
