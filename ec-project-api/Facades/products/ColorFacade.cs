using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Services;
using System;

namespace ec_project_api.Facades.products
{
    public class ColorFacade
    {
        private readonly IColorService _colorService;
        private readonly IMapper _mapper;

        public ColorFacade(IColorService colorService, IMapper mapper)
        {
            _colorService = colorService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ColorDto>> GetAllAsync()
        {
            var colors = await _colorService.GetAllAsync();
            return _mapper.Map<IEnumerable<ColorDto>>(colors);
        }

        public async Task<ColorDetailDto> GetByIdAsync(short id)
        {
            var color = await _colorService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(ColorMessages.ColorNotFound);

            return _mapper.Map<ColorDetailDto>(color);
        }

        public async Task<bool> CreateAsync(ColorCreateRequest request)
        {
            var existing = await _colorService.FirstOrDefaultAsync(c => c.Name == request.Name.Trim());
            if (existing != null)
                throw new InvalidOperationException(ColorMessages.ColorNameAlreadyExists);

            var color = _mapper.Map<Color>(request);
            color.CreatedAt = DateTime.UtcNow;
            color.UpdatedAt = DateTime.UtcNow;

            return await _colorService.CreateAsync(color);
        }

        public async Task<bool> UpdateAsync(short id, ColorUpdateRequest request)
        {
            var existing = await _colorService.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException(ColorMessages.ColorNotFound);

            // Kiểm tra trùng tên với màu khác
            var duplicate = await _colorService.FirstOrDefaultAsync(c => c.ColorId != id && c.Name == request.Name.Trim());
            if (duplicate != null)
                throw new InvalidOperationException(ColorMessages.ColorNameAlreadyExists);

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            return await _colorService.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(short id)
        {
            var color = await _colorService.GetByIdAsync(id);
            if (color == null)
                throw new KeyNotFoundException(ColorMessages.ColorNotFound);

            return await _colorService.DeleteAsync(color);
        }
    }
}