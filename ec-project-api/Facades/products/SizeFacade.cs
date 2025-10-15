using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services;
using System.Linq.Expressions;

namespace ec_project_api.Facades.products
{
    public class SizeFacade
    {
        private readonly ISizeService _sizeService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public SizeFacade(ISizeService sizeService, IStatusService statusService, IMapper mapper)
        {
            _sizeService = sizeService;
            _statusService = statusService;
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

            var draftStatus = await _statusService.FirstOrDefaultAsync(s => s.EntityType == "Size" && s.Name == "Draft")
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var size = _mapper.Map<Size>(request);
            size.StatusId = draftStatus.StatusId;
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

            var status = await _statusService.GetByIdAsync(request.StatusId);
            if (status == null || status.EntityType != "Size")
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            return await _sizeService.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(byte id)
        {
            var size = await _sizeService.GetByIdAsync(id);
            if (size == null)
                throw new KeyNotFoundException(SizeMessages.SizeNotFound);

            if (size.Status.Name != "Draft")
                throw new InvalidOperationException(SizeMessages.SizeDeleteFailedNotDraft);

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

        public async Task<PagedResult<SizeDto>> GetAllPagedAsync(SizeFilter filter)
        {
            var options = new QueryOptions<Size>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
            };

            options.Filter = BuildSizeFilter(filter);

            var pagedResult = await _sizeService.GetAllPagedAsync(options);

            var dtoList = _mapper.Map<IEnumerable<SizeDto>>(pagedResult.Items);
            var pagedResultDto = new PagedResult<SizeDto>
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