using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Dtos.response.pagination;
using System;
using System.Linq.Expressions;
using ec_project_api.Repository.Base;
using ec_project_api.Constants.Messages;

namespace ec_project_api.Facades.products {
    public class ColorFacade {
        private readonly IColorService _colorService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public ColorFacade(IColorService colorService, IStatusService statusService, IMapper mapper) {
            _colorService = colorService;
            _statusService = statusService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ColorDto>> GetAllAsync() {
            var colors = await _colorService.GetAllAsync();
            return _mapper.Map<IEnumerable<ColorDto>>(colors);
        }

        public async Task<ColorDetailDto> GetByIdAsync(short id) {
            var color = await _colorService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(ColorMessages.ColorNotFound);

            return _mapper.Map<ColorDetailDto>(color);
        }

        public async Task<bool> CreateAsync(ColorCreateRequest request) {
            var existing = await _colorService.FirstOrDefaultAsync(c => c.Name == request.Name.Trim());
            if (existing != null)
                throw new InvalidOperationException(ColorMessages.ColorNameAlreadyExists);

            var draftStatus = await _statusService.FirstOrDefaultAsync(s => s.Name == "Draft" && s.EntityType == "Color")
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var color = _mapper.Map<Color>(request);
            color.StatusId = draftStatus.StatusId;
            color.CreatedAt = DateTime.UtcNow;
            color.UpdatedAt = DateTime.UtcNow;

            return await _colorService.CreateAsync(color);
        }

        public async Task<bool> UpdateAsync(short id, ColorUpdateRequest request) {
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

        public async Task<bool> DeleteAsync(short id) {
            var color = await _colorService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(ColorMessages.ColorNotFound);

            if (color.Status.Name != "Draft")
                throw new InvalidOperationException(ColorMessages.ColorDeleteFailedNotDraft);

            return await _colorService.DeleteAsync(color);
        }

        private static Expression<Func<Color, bool>> BuildColorFilter(ColorFilter filter) {
            return c =>
                (string.IsNullOrEmpty(filter.StatusName) ||
                    (c.Status != null && c.Status.Name == filter.StatusName && c.Status.EntityType == "Color")) &&
                (string.IsNullOrEmpty(filter.Search) ||
                    c.Name.Contains(filter.Search) ||
                    c.ColorId.ToString().Contains(filter.Search));
        }

        public async Task<PagedResult<ColorDto>> GetAllPagedAsync(ColorFilter filter) {
            var options = new QueryOptions<Color>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Filter = BuildColorFilter(filter)
            };

            var pagedResult = await _colorService.GetAllPagedAsync(options);

            var dtoList = _mapper.Map<IEnumerable<ColorDto>>(pagedResult.Items);
            var pagedResultDto = new PagedResult<ColorDto>
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