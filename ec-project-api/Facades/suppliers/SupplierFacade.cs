using AutoMapper;
using ec_project_api.Dtos.request.suppliers;
using ec_project_api.Dtos.response.suppliers;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Constants.variables;
using ec_project_api.Constants.Messages;
using ec_project_api.Interfaces.Suppliers;

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

        public async Task<IEnumerable<SupplierDto>> GetAllAsync()
        {
            var suppliers = await _supplierService.GetAllAsync();
            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
        }

        public async Task<ec_project_api.Dtos.response.pagination.PagedResult<SupplierDto>> GetAllPagedAsync(SupplierFilter filter)
        {
            var options = new ec_project_api.Repository.Base.QueryOptions<Models.Supplier>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            options.Filter = s =>
                (!filter.StatusId.HasValue || s.StatusId == filter.StatusId.Value) &&
                (string.IsNullOrEmpty(filter.Name) || s.Name.Contains(filter.Name));

            if (!string.IsNullOrEmpty(filter.OrderBy))
            {
                switch (filter.OrderBy)
                {
                    case "name_asc":
                        options.OrderBy = q => q.OrderBy(s => s.Name);
                        break;
                    case "name_desc":
                        options.OrderBy = q => q.OrderByDescending(s => s.Name);
                        break;
                    case "createdAt_asc":
                        options.OrderBy = q => q.OrderBy(s => s.CreatedAt); 
                        break;
                    case "createdAt_desc":
                        options.OrderBy = q => q.OrderByDescending(s => s.CreatedAt); 
                        break;
                }
            }
            options.Includes.Add(s => s.Status);

            var paged = await _supplierService.GetAllPagedAsync(options);

            var dtoItems = _mapper.Map<IEnumerable<SupplierDto>>(paged.Items);
            var pagedDto = new ec_project_api.Dtos.response.pagination.PagedResult<SupplierDto>
            {
                Items = dtoItems,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };

            return pagedDto;
        }

        public async Task<SupplierDto?> GetByIdAsync(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
                throw new InvalidOperationException(SupplierMessages.SupplierNotFound);

            return _mapper.Map<SupplierDto>(supplier);
        }

        public async Task<bool> CreateAsync(SupplierCreateRequest request)
        {
            var draftStatus = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.Supplier && s.Name == StatusVariables.Draft
            );

            if (draftStatus == null)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var supplier = _mapper.Map<Supplier>(request);
            supplier.StatusId = draftStatus.StatusId;
            supplier.CreatedAt = DateTime.UtcNow;

            var result = await _supplierService.CreateAsync(supplier);
            if (!result)
                throw new InvalidOperationException(SupplierMessages.CreateFailed);

            return result;
        }

        public async Task<bool> UpdateAsync(int id, SupplierUpdateRequest request)
        {
            var existing = await _supplierService.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException(SupplierMessages.SupplierNotFound);

            if (request.StatusId != 0)
            {
                var status = await _statusService.GetByIdAsync(request.StatusId);
                if (status == null)
                    throw new InvalidOperationException(StatusMessages.StatusNotFound);
            }

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            var result = await _supplierService.UpdateAsync(existing);
            if (!result)
                throw new InvalidOperationException(SupplierMessages.UpdateFailed);

            return result;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _supplierService.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException(SupplierMessages.SupplierNotFound);

            var inactiveStatus = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.Supplier && s.Name == StatusVariables.Inactive
            );

            if (inactiveStatus == null)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var result = await _supplierService.DeleteAsync(existing, inactiveStatus.StatusId);
            if (!result)
                throw new InvalidOperationException(SupplierMessages.DeleteFailed);

            return result;
        }

        public async Task<bool> UpdateStatusAsync(int id, short newStatusId)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
                throw new InvalidOperationException(SupplierMessages.SupplierNotFound);

            var status = await _statusService.GetByIdAsync(newStatusId);
            if (status == null)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var result = await _supplierService.UpdateStatusAsync(id, newStatusId);
            if (!result)
                throw new InvalidOperationException(SupplierMessages.UpdateStatusFailed);

            return result;
        }
    }
}
