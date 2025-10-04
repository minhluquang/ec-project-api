using AutoMapper;
using ec_project_api.Dtos.request.suppliers;
using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.suppliers;
using ec_project_api.Interfaces.Suppliers;
using ec_project_api.Models;

namespace ec_project_api.Facades.Suppliers
{
    public class SupplierFacade
    {
        private readonly ISupplierService _supplierService;
        private readonly IMapper _mapper;

        public SupplierFacade(ISupplierService supplierService, IMapper mapper)
        {
            _supplierService = supplierService;
            _mapper = mapper;
        }

        public async Task<ResponseData<IEnumerable<SupplierDto>>> GetAllAsync()
        {
            var suppliers = await _supplierService.GetAllAsync();
            var result = _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
            return ResponseData<IEnumerable<SupplierDto>>.Success(StatusCodes.Status200OK, result);
        }

        public async Task<ResponseData<SupplierDto>> GetByIdAsync(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
                return ResponseData<SupplierDto>.Error(StatusCodes.Status404NotFound, "Không tìm thấy nhà cung cấp.");

            var result = _mapper.Map<SupplierDto>(supplier);
            return ResponseData<SupplierDto>.Success(StatusCodes.Status200OK, result);
        }

        public async Task<ResponseData<SupplierDto>> CreateAsync(SupplierCreateRequest request)
        {
            var supplier = _mapper.Map<Supplier>(request);
            var success = await _supplierService.CreateAsync(supplier);

            if (!success)
                return ResponseData<SupplierDto>.Error(StatusCodes.Status400BadRequest, "Không thể tạo nhà cung cấp.");

            var dto = _mapper.Map<SupplierDto>(supplier);
            return ResponseData<SupplierDto>.Success(StatusCodes.Status201Created, dto, "Nhà cung cấp đã được tạo thành công.");
        }

        public async Task<ResponseData<bool>> UpdateAsync(int id, SupplierUpdateRequest request)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
                return ResponseData<bool>.Error(StatusCodes.Status404NotFound, "Không tìm thấy nhà cung cấp.");

            _mapper.Map(request, supplier);
            var success = await _supplierService.UpdateAsync(supplier);

            if (!success)
                return ResponseData<bool>.Error(StatusCodes.Status400BadRequest, "Không thể cập nhật nhà cung cấp.");

            return ResponseData<bool>.Success(StatusCodes.Status200OK, true, "Cập nhật nhà cung cấp thành công.");
        }

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
       public async Task<ResponseData<PagedResponse<SupplierDto>>> GetPagedAsync(SupplierQueryRequest filter)
        {
            var pagedSuppliers = await _supplierService.GetPagedAsync(filter);

            var result = new PagedResponse<SupplierDto>(
                _mapper.Map<IEnumerable<SupplierDto>>(pagedSuppliers.Items),
                pagedSuppliers.TotalItems,
                pagedSuppliers.Page,
                pagedSuppliers.PageSize
            );

            return ResponseData<PagedResponse<SupplierDto>>.Success(StatusCodes.Status200OK, result);
        }
    }
    
}
