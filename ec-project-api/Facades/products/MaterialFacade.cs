using AutoMapper;
using ec_project_api.Constants.messages; // Ensure MaterialMessages file exists
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.materials;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services; // Ensure you have IMaterialService
using ec_project_api.Services.products;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ec_project_api.Facades.materials // Updated namespace for Facade
{
    public class MaterialFacade
    {
        private readonly IMaterialService _materialService;
        private readonly IProductService _productService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public MaterialFacade(IMaterialService materialService, IProductService productService, IStatusService statusService, IMapper mapper)
        {
            _materialService = materialService;
            _productService = productService;
            _statusService = statusService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MaterialDto>> GetAllAsync()
        {
            var materials = await _materialService.GetAllAsync();
            return _mapper.Map<IEnumerable<MaterialDto>>(materials);
        }

        public async Task<MaterialDetailDto> GetByIdAsync(short id)
        {
            var material = await _materialService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(MaterialMessages.MaterialNotFound);

            return _mapper.Map<MaterialDetailDto>(material);
        }

        public async Task<bool> CreateAsync(MaterialCreateRequest request)
        {
            var existing = await _materialService.FirstOrDefaultAsync(m => m.Name == request.Name.Trim());
            if (existing != null)
                throw new InvalidOperationException(MaterialMessages.MaterialAlreadyExists);

            var inActiveStatus = await _statusService.FirstOrDefaultAsync(s => s.Name == "Inactive" && s.EntityType == "Material")
               ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);


            var material = _mapper.Map<Material>(request);
            material.StatusId = inActiveStatus.StatusId;
            material.CreatedAt = DateTime.UtcNow;
            material.UpdatedAt = DateTime.UtcNow;

            return await _materialService.CreateAsync(material);
        }

        public async Task<bool> UpdateAsync(short id, MaterialUpdateRequest request)
        {
            var existing = await _materialService.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException(MaterialMessages.MaterialNotFound);

            // Check for duplicate name with another material
            var duplicate = await _materialService.FirstOrDefaultAsync(m => m.MaterialId != id && m.Name == request.Name.Trim());
            if (duplicate != null)
                throw new InvalidOperationException(MaterialMessages.MaterialAlreadyExists);

            var existingStatus = await _statusService.GetByIdAsync(request.StatusId);
            if (existingStatus == null || existingStatus.EntityType != EntityVariables.Material)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            return await _materialService.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(short id)
        {
            var material = await _materialService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(MaterialMessages.MaterialNotFound);

            // Kiểm tra trạng thái chất liệu
            if (material.Status.Name != "Inactive")
            {
                throw new InvalidOperationException(MaterialMessages.MaterialDeleteFailedNotInActive);
            }

            var currentProducts = await _productService.GetAllAsync();
            // Kiểm tra xem có sản phẩm nào sử dụng màu sắc này không
            if (material.Products.Any(p => currentProducts.Any(cp => cp.ProductId == p.ProductId)))
            {
                throw new InvalidOperationException(MaterialMessages.MaterialInUse);
            }

            return await _materialService.DeleteAsync(material);
        }

        private static Expression<Func<Material, bool>> BuildMaterialFilter(MaterialFilter filter)
        {
            return m =>
                (string.IsNullOrEmpty(filter.StatusName) || m.Status.Name == filter.StatusName) &&
                (string.IsNullOrEmpty(filter.Search) ||
                 m.Name.Contains(filter.Search) ||
                m.Description.Contains(filter.Search) ||
                m.MaterialId.ToString().Contains(filter.Search));
        }

        public async Task<PagedResult<MaterialDetailDto>> GetAllPagedAsync(MaterialFilter filter)
        {
            var options = new QueryOptions<Material>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Filter = BuildMaterialFilter(filter)
            };

            var pagedResult = await _materialService.GetAllPagedAsync(options);

            var dtoList = _mapper.Map<IEnumerable<MaterialDetailDto>>(pagedResult.Items);
            var pagedResultDto = new PagedResult<MaterialDetailDto>
            {
                Items = dtoList,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
            return pagedResultDto;
        }
    }
}