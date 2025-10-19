using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.response.discounts;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services;
using ec_project_api.Services.discounts;
using System.Linq.Expressions;

namespace ec_project_api.Facades.discounts
{
    public class DiscountFacade
    {
        private readonly IDiscountService _discountService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public DiscountFacade(IDiscountService discountService, IStatusService statusService, IMapper mapper)
        {
            _discountService = discountService;
            _statusService = statusService; // keep naming consistent with other facades if you used _status_service elsewhere; otherwise change to _statusService
            _mapper = mapper;
        }

        public async Task<IEnumerable<DiscountDto>> GetAllAsync()
        {
            var discounts = await _discountService.GetAllAsync();
            return _mapper.Map<IEnumerable<DiscountDto>>(discounts);
        }

        public async Task<DiscountDetailDto> GetByIdAsync(int id)
        {
            var discount = await _discountService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(DiscountMessages.DiscountNotFound);

            return _mapper.Map<DiscountDetailDto>(discount);
        }

        public async Task<bool> CreateAsync(DiscountCreateRequest request)
        {
            var existing = await _discountService.FirstOrDefaultAsync(d => d.Code == request.Code.Trim());
            if (existing != null)
                throw new InvalidOperationException(DiscountMessages.DiscountCodeAlreadyExists);

            // follow ColorFacade pattern: set draft status on create
            var draftStatus = await _statusService.FirstOrDefaultAsync(s => s.Name == StatusVariables.Draft && s.EntityType == EntityVariables.Discount)
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var discount = _mapper.Map<Discount>(request);
            discount.StatusId = draftStatus.StatusId;
            discount.CreatedAt = DateTime.UtcNow;
            discount.UpdatedAt = DateTime.UtcNow;

            return await _discountService.CreateAsync(discount);
        }

        public async Task<bool> UpdateAsync(int id, DiscountUpdateRequest request)
        {
            var existing = await _discountService.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException(DiscountMessages.DiscountNotFound);

            var duplicate = await _discountService.FirstOrDefaultAsync(d => d.DiscountId != id && d.Code == request.Code.Trim());
            if (duplicate != null)
                throw new InvalidOperationException(DiscountMessages.DiscountCodeAlreadyExists);

            // follow ColorFacade: simple map and update timestamp (no strict status entity-type check here)
            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            return await _discountService.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var discount = await _discountService.GetByIdAsync(id);
            if (discount == null)
                throw new KeyNotFoundException(DiscountMessages.DiscountNotFound);

            if (discount.Status.Name != StatusVariables.Draft)
                throw new InvalidOperationException(DiscountMessages.DiscountDeleteFailedNotDraft);

            return await _discountService.DeleteAsync(discount);
        }

        private static Expression<Func<Discount, bool>> BuildDiscountFilter(DiscountFilter filter)
        {
            return d =>
                (string.IsNullOrEmpty(filter.StatusName) ||
                    (d.Status != null && d.Status.Name == filter.StatusName && d.Status.EntityType == EntityVariables.Discount)) &&
                (string.IsNullOrEmpty(filter.Search) ||
                    d.Code.Contains(filter.Search) ||
                    (d.Description != null && d.Description.Contains(filter.Search)) ||
                    d.DiscountId.ToString().Contains(filter.Search));
        }

        public async Task<PagedResult<DiscountDetailDto>> GetAllPagedAsync(DiscountFilter filter)
        {
            var options = new QueryOptions<Discount>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Filter = BuildDiscountFilter(filter)
            };

            var pagedResult = await _discountService.GetAllPagedAsync(options);

            var dtoList = _mapper.Map<IEnumerable<DiscountDetailDto>>(pagedResult.Items);
            var pagedResultDto = new PagedResult<DiscountDetailDto>
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