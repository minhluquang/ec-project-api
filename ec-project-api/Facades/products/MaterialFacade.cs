using AutoMapper;
using ec_project_api.Constants.messages; // Giả sử bạn sẽ tạo file MaterialMessages
using ec_project_api.Dtos.request.materials;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Services; // Giả sử bạn có IMaterialService
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ec_project_api.Facades.materials // Thay đổi namespace cho Facade
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

        public async Task<MaterialDetailDto?> GetByIdAsync(short id)
        {
            var material = await _materialService.GetByIdAsync(id);
            if (material == null)
                throw new KeyNotFoundException(MaterialMessages.MaterialNotFound); // Thay đổi thông báo lỗi

            return _mapper.Map<MaterialDetailDto>(material);
        }

        public async Task<bool> CreateAsync(MaterialCreateRequest request)
        {
            var existing = await _materialService.FirstOrDefaultAsync(m => m.Name == request.Name.Trim());
            if (existing != null)
                throw new InvalidOperationException(MaterialMessages.MaterialAlreadyExists); // Thay đổi thông báo lỗi

            var material = _mapper.Map<Material>(request);
            material.CreatedAt = DateTime.UtcNow;
            material.UpdatedAt = DateTime.UtcNow;

            return await _materialService.CreateAsync(material);
        }

        public async Task<bool> UpdateAsync(short id, MaterialUpdateRequest request)
        {
            var existing = await _materialService.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException(MaterialMessages.MaterialNotFound); // Thay đổi thông báo lỗi

            // Kiểm tra trùng tên với chất liệu khác
            var duplicate = await _materialService.FirstOrDefaultAsync(m => m.MaterialId != id && m.Name == request.Name.Trim());
            if (duplicate != null)
                throw new InvalidOperationException(MaterialMessages.MaterialAlreadyExists); // Thay đổi thông báo lỗi

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            return await _materialService.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(short id)
        {
            var material = await _materialService.GetByIdAsync(id);
            if (material == null)
                throw new KeyNotFoundException(MaterialMessages.MaterialNotFound); // Thay đổi thông báo lỗi

            return await _materialService.DeleteAsync(material);
        }
    }
}