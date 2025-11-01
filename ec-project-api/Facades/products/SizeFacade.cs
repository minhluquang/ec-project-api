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
using System.Linq.Expressions;

namespace ec_project_api.Facades.products
{
    public class SizeFacade
    {
        private readonly ISizeService _sizeService;
        private readonly IProductVariantService _productVariantService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public SizeFacade(ISizeService sizeService, IProductVariantService productVariantService, IStatusService statusService, IMapper mapper)
        {
            _sizeService = sizeService;
            _productVariantService = productVariantService;
            _statusService = statusService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SizeDto>> GetSizeOptionsAsync()
        {
            var sizes = await _sizeService.GetSizeOptionsAsync();
            return _mapper.Map<IEnumerable<SizeDto>>(sizes);
        }

        public async Task<SizeDetailDto> GetByIdAsync(short id)
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

            var inactiveStatus = await _statusService.FirstOrDefaultAsync(s => s.EntityType == "Size" && s.Name == "Inactive")
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var size = _mapper.Map<Size>(request);
            size.StatusId = inactiveStatus.StatusId;
            size.CreatedAt = DateTime.UtcNow;
            size.UpdatedAt = DateTime.UtcNow;

            return await _sizeService.CreateAsync(size);
        }

        public async Task<bool> UpdateAsync(short id, SizeUpdateRequest request)
        {
            var existing = await _sizeService.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException(SizeMessages.SizeNotFound);

            var duplicate = await _sizeService.FirstOrDefaultAsync(s => s.SizeId != id && s.Name == request.Name.Trim());
            if (duplicate != null)
                throw new InvalidOperationException(SizeMessages.SizeNameAlreadyExists);

            var existingStatus = await _statusService.GetByIdAsync(request.StatusId);
            if (existingStatus == null || existingStatus.EntityType != EntityVariables.Size)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            return await _sizeService.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(short id)
        {
            var size = await _sizeService.GetByIdAsync(id);
            if (size == null)
                throw new KeyNotFoundException(SizeMessages.SizeNotFound);

            if (size.Status.Name != "Inactive")
                throw new InvalidOperationException(SizeMessages.SizeDeleteFailedNotInActive);

            var currentProductVariants = await _productVariantService.GetAllAsync();

            // Kiểm tra xem có sản phẩm nào sử dụng kích thước này không
            if (currentProductVariants.Any(pv => pv.SizeId == size.SizeId))
            {
                throw new InvalidOperationException(SizeMessages.SizeInUse);
            }


            return await _sizeService.DeleteAsync(size);
        }

        private static Expression<Func<Size, bool>> BuildSizeFilter(SizeFilter filter)
        {
            return s =>
                (string.IsNullOrEmpty(filter.StatusName) ||
                    (s.Status != null && s.Status.Name == filter.StatusName && s.Status.EntityType == "Size")) &&
                (string.IsNullOrEmpty(filter.Search) ||
                    s.Name.Contains(filter.Search) ||
                    s.SizeId.ToString().Contains(filter.Search));
        }

        public async Task<PagedResult<SizeDetailDto>> GetAllPagedAsync(SizeFilter filter)
        {
            var options = new QueryOptions<Size>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
            };

            options.Filter = BuildSizeFilter(filter);

            var pagedResult = await _sizeService.GetAllPagedAsync(options);

            var dtoList = _mapper.Map<IEnumerable<SizeDetailDto>>(pagedResult.Items);
            var pagedResultDto = new PagedResult<SizeDetailDto>
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