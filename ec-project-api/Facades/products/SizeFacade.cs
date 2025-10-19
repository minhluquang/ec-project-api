using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Services;
using System;

namespace ec_project_api.Facades.products
{
    public class SizeFacade
    {
        private readonly ISizeService _sizeService;
        private readonly IMapper _mapper;

        public SizeFacade(ISizeService sizeService, IMapper mapper)
        {
            _sizeService = sizeService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SizeDto>> GetAllAsync()
        {
            var sizes = await _sizeService.GetAllAsync();
            return _mapper.Map<IEnumerable<SizeDto>>(sizes);
        }

        public async Task<SizeDetailDto> GetByIdAsync(byte id)
        {
            var size = await _sizeService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(SizeMessages.SizeNotFound);

            return _mapper.Map<SizeDetailDto>(size);
        }

        public async Task<bool> CreateAsync(SizeCreateRequest request)
        {
            var existing = await _sizeService.FirstOrDefaultAsync(s => s.Name == request.Name.Trim());
            if (existing != null)
                throw new InvalidOperationException(SizeMessages.SizeNameAlreadyExists);

            var size = _mapper.Map<Size>(request);
            size.CreatedAt = DateTime.UtcNow;
            size.UpdatedAt = DateTime.UtcNow;

            return await _sizeService.CreateAsync(size);
        }

        public async Task<bool> UpdateAsync(byte id, SizeUpdateRequest request)
        {
            var existing = await _sizeService.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException(SizeMessages.SizeNotFound);

            var duplicate = await _sizeService.FirstOrDefaultAsync(s => s.SizeId != id && s.Name == request.Name.Trim());
            if (duplicate != null)
                throw new InvalidOperationException(SizeMessages.SizeNameAlreadyExists);

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            return await _sizeService.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(byte id)
        {
            var size = await _sizeService.GetByIdAsync(id);
            if (size == null)
                throw new KeyNotFoundException(SizeMessages.SizeNotFound);

            return await _sizeService.DeleteAsync(size);
        }
    }
}