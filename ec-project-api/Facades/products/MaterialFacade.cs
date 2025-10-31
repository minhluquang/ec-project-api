using AutoMapper;
using ec_project_api.Constants.messages; // Ensure MaterialMessages file exists
using ec_project_api.Dtos.request.materials;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services; // Ensure you have IMaterialService
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ec_project_api.Facades.materials // Updated namespace for Facade
{
    public class MaterialFacade
    {
        private readonly IMaterialService _materialService;
        private readonly IMapper _mapper;

        public MaterialFacade(IMaterialService materialService, IMapper mapper)
        {
            _materialService = materialService;
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

            var material = _mapper.Map<Material>(request);
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

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            return await _materialService.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(short id)
        {
            var material = await _materialService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(MaterialMessages.MaterialNotFound);

            return await _materialService.DeleteAsync(material);
        }

        private static Expression<Func<Material, bool>> BuildMaterialFilter(MaterialFilter filter)
        {
            return m =>
                (string.IsNullOrEmpty(filter.StatusName) || m.Status.Name == filter.StatusName) &&
                (string.IsNullOrEmpty(filter.Search) ||
                 m.Name.Contains(filter.Search) ||
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