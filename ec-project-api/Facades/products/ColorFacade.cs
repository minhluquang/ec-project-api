using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services;
using ec_project_api.Services.products;
using System;
using System.Linq.Expressions;

namespace ec_project_api.Facades.products {
    public class ColorFacade {
        private readonly IColorService _colorService;
        private readonly IProductService _productService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public ColorFacade(IColorService colorService, IProductService productService, IStatusService statusService, IMapper mapper) {
            _colorService = colorService;
            _productService = productService;
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

            var inActiveStatus = await _statusService.FirstOrDefaultAsync(s => s.Name == "Inactive" && s.EntityType == "Color")
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var color = _mapper.Map<Color>(request);
            color.StatusId = inActiveStatus.StatusId;
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

            var existingStatus = await _statusService.GetByIdAsync(request.StatusId);
            if (existingStatus == null || existingStatus.EntityType != EntityVariables.Color)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            if (existingStatus.Name != StatusVariables.Inactive)
            {
                // Kiểm tra có sản phẩm nào đang active mà thuộc về product group này không
                if (existing.Products != null && existing.Products.Any(p => p.Status.EntityType == EntityVariables.Product && p.Status.Name == StatusVariables.Active))
                {
                    throw new InvalidOperationException(ColorMessages.ColorUpdateStatusFailedProductActive);
                }

            }


            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            return await _colorService.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(short id)
        {
            // Kiểm tra màu sắc theo ID
            var color = await _colorService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(ColorMessages.ColorNotFound);

            // Kiểm tra trạng thái màu sắc
            if (color.Status.Name != "Inactive")
            {
                throw new InvalidOperationException(ColorMessages.ColorDeleteFailedNotInActive);
            }

            var currentProducts = await _productService.GetAllAsync();
            // Kiểm tra xem có sản phẩm nào sử dụng màu sắc này không
            if (color.Products.Any(p => currentProducts.Any(cp => cp.ProductId == p.ProductId)))
            {
                throw new InvalidOperationException(ColorMessages.ColorInUse);
            }

            // Thực hiện xóa màu sắc
            return await _colorService.DeleteAsync(color);
        }

        private static Expression<Func<Color, bool>> BuildColorFilter(ColorFilter filter) {
            return c =>
                (string.IsNullOrEmpty(filter.StatusName) ||
                    (c.Status != null && c.Status.Name == filter.StatusName && c.Status.EntityType == "Color")) &&
                (string.IsNullOrEmpty(filter.Search) ||
                    c.DisplayName.Contains(filter.Search) ||
                    c.Name.Contains(filter.Search) ||
                    c.ColorId.ToString().Contains(filter.Search));
        }

        public async Task<PagedResult<ColorDetailDto>> GetAllPagedAsync(ColorFilter filter) {
            var options = new QueryOptions<Color>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Filter = BuildColorFilter(filter)
            };

            var pagedResult = await _colorService.GetAllPagedAsync(options);

            var dtoList = _mapper.Map<IEnumerable<ColorDetailDto>>(pagedResult.Items);
            var pagedResultDto = new PagedResult<ColorDetailDto>
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