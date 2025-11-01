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
            // Trim code
            request.Code = request.Code?.Trim().ToUpper();

            // 1. Kiểm tra mã khuyến mãi trùng (chỉ trùng nếu code đang Active)
            var existing = await _discountService.FirstOrDefaultAsync(
                d => d.Code == request.Code && d.Status.Name == StatusVariables.Active);
            if (existing != null)
                throw new InvalidOperationException(DiscountMessages.DiscountCodeAlreadyExists);

            // 2. Validate MinOrderAmount
            if (request.MinOrderAmount < 0)
                throw new InvalidOperationException("Giá trị đơn hàng tối thiểu phải lớn hơn hoặc bằng 0.");

            // 3. Validate DiscountValue
            if (request.DiscountValue <= 0)
                throw new InvalidOperationException("Giá trị khuyến mãi phải lớn hơn 0.");

            if (request.DiscountType == "percentage" &&
                (request.DiscountValue <= 0 || request.DiscountValue >= 100))
            {
                throw new InvalidOperationException("Giá trị phần trăm phải từ 0 đến 100.");
            }

            // Nếu DiscountType = fixed, MaxDiscountAmount phải bằng DiscountValue
            if (request.DiscountType == "fixed")
            {
                if (!request.MaxDiscountAmount.HasValue || request.MaxDiscountAmount.Value != request.DiscountValue)
                    throw new InvalidOperationException("Giá trị giảm tối đa phải bằng giá khuyến mãi.");
            }

            // 4. Validate MaxDiscountAmount
            if (request.MaxDiscountAmount.HasValue)
            {
                if (request.MaxDiscountAmount.Value < 0)
                    throw new InvalidOperationException("Giá trị giảm tối đa phải lớn hơn hoặc bằng 0.");

                if (request.MaxDiscountAmount.Value > request.MinOrderAmount)
                    throw new InvalidOperationException("Giá trị giảm tối đa phải nhỏ hơn hoặc bằng giá trị đơn hàng tối thiểu.");
            }

            // 5. Validate UsageLimit - null hoặc >= 1
            if (request.UsageLimit.HasValue && request.UsageLimit.Value < 1)
                throw new InvalidOperationException("Giới hạn sử dụng phải lớn hơn 0.");

            // Set draft status on create
            var inActiveStatus = await _statusService.FirstOrDefaultAsync(
                s => s.Name == StatusVariables.Inactive && s.EntityType == EntityVariables.Discount)
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var discount = _mapper.Map<Discount>(request);
            discount.StatusId = inActiveStatus.StatusId;
            discount.UsedCount = 0;
            discount.CreatedAt = DateTime.UtcNow;
            discount.UpdatedAt = DateTime.UtcNow;

            return await _discountService.CreateAsync(discount);
        }

        public async Task<bool> UpdateAsync(int id, DiscountUpdateRequest request)
        {
            var existing = await _discountService.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException(DiscountMessages.DiscountNotFound);

            if (existing.UsedCount > 0 || (existing.EndAt.HasValue && existing.EndAt.Value.Date < DateTime.UtcNow.Date))
            {
                throw new InvalidOperationException("Không thể cập nhật mã khuyến mãi đã hết hạn hoặc đã sử dụng.");
            }


            // Kiểm tra code trùng và cho phép sửa code chỉ khi usedCount == 0
            if (existing.UsedCount == 0)
            {
                var duplicate = await _discountService.FirstOrDefaultAsync(
                    d => d.DiscountId != id && d.Code == request.Code.Trim().ToUpper() && d.Status.Name == StatusVariables.Active);
                if (duplicate != null)
                    throw new InvalidOperationException(DiscountMessages.DiscountCodeAlreadyExists);

                existing.Code = request.Code.Trim().ToUpper(); // Gán code mới
            }
            else
            {
                // Nếu đã dùng rồi, code không thể sửa
                if (request.Code.Trim() != existing.Code)
                    throw new InvalidOperationException("Mã khuyến mãi đã được sử dụng, không thể thay đổi mã.");
            }


            // Validate MinOrderAmount
            if (request.MinOrderAmount < 0)
                throw new InvalidOperationException("Giá trị đơn hàng tối thiểu phải lớn hơn hoặc bằng 0.");

            // Validate DiscountValue
            if (request.DiscountValue <= 0)
                throw new InvalidOperationException("Giá trị khuyến mãi phải lớn hơn 0.");

            if (request.DiscountType == "percentage" &&
                (request.DiscountValue <= 0 || request.DiscountValue > 100))
            {
                throw new InvalidOperationException("Giá trị phần trăm phải từ 0 đến 100.");
            }

            // Nếu DiscountType = fixed, MaxDiscountAmount phải bằng DiscountValue
            if (request.DiscountType == "fixed")
            {
                if (!request.MaxDiscountAmount.HasValue || request.MaxDiscountAmount.Value != request.DiscountValue)
                    throw new InvalidOperationException("Giá trị giảm tối đa phải bằng giá khuyến mãi.");
            }

            // Validate MaxDiscountAmount
            if (request.MaxDiscountAmount.HasValue)
            {
                if (request.MaxDiscountAmount.Value < 0)
                    throw new InvalidOperationException("Giá trị giảm tối đa phải lớn hơn hoặc bằng 0.");

                if (request.MaxDiscountAmount.Value > request.MinOrderAmount)
                    throw new InvalidOperationException("Giá trị giảm tối đa phải nhỏ hơn hoặc bằng giá trị đơn hàng tối thiểu.");
            }

            // Validate UsageLimit
            if (request.UsageLimit.HasValue && request.UsageLimit.Value < existing.UsedCount)
                throw new InvalidOperationException($"Giới hạn sử dụng phải lớn hơn hoặc bằng số lần đã dùng ({existing.UsedCount}).");

            // Kiểm tra ngày bắt đầu
            if (existing.UsedCount == 0)
            {
                if (request.StartAt.HasValue && request.StartAt.Value.Date < DateTime.UtcNow.Date)
                    throw new InvalidOperationException("Ngày bắt đầu phải bằng hoặc sau hôm nay.");
                existing.StartAt = request.StartAt;
            }

            // Ngày kết thúc
            if (request.EndAt.HasValue)
            {
                if (existing.StartAt.HasValue && request.EndAt.Value.Date < existing.StartAt.Value.Date)
                    throw new InvalidOperationException("Ngày kết thúc phải bằng hoặc sau ngày bắt đầu.");
                existing.EndAt = request.EndAt;
            }

            var existingStatus = await _statusService.GetByIdAsync(request.StatusId);
            if (existingStatus == null || existingStatus.EntityType != EntityVariables.Discount)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);


            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;
            return await _discountService.UpdateAsync(existing);
        }



        public async Task<bool> DeleteAsync(int id)
        {
            var discount = await _discountService.GetByIdAsync(id);
            if (discount == null)
                throw new KeyNotFoundException(DiscountMessages.DiscountNotFound);

            if (discount.Status.Name != StatusVariables.Inactive)
                throw new InvalidOperationException(DiscountMessages.DiscountDeleteFailedNotInActive);
            if (discount.UsedCount > 0)
                throw new InvalidOperationException(DiscountMessages.DiscountUsed);

            return await _discountService.DeleteAsync(discount);
        }

        private static Expression<Func<Discount, bool>> BuildDiscountFilter(DiscountFilter filter)
        {
            return d =>
                (string.IsNullOrEmpty(filter.StatusName) ||
                    (d.Status != null && d.Status.Name == filter.StatusName && d.Status.EntityType == EntityVariables.Discount)) &&
                (string.IsNullOrEmpty(filter.Search) ||
                    d.Code.Contains(filter.Search) ||
                    (d.Description != null && d.Description.Contains(filter.Search))) &&
                (string.IsNullOrEmpty(filter.DiscountType) || d.DiscountType == filter.DiscountType);
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

        public async Task<bool> CheckAndUpdateDiscountStatusAsync(int discountId)
        {
            var inactiveStatus = await _statusService.FirstOrDefaultAsync(
                s => s.Name == StatusVariables.Inactive && s.EntityType == EntityVariables.Discount)
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

            return await _discountService.CheckAndUpdateDiscountStatusByIdAsync(discountId, inactiveStatus.StatusId);
        }

        public async Task<int> CheckAndUpdateAllActiveDiscountsAsync()
        {
            // Lấy tất cả discount có trạng thái Active
            var activeDiscounts = await _discountService.FindAsync(d =>
                d.Status.Name == StatusVariables.Active);

            if (!activeDiscounts.Any())
                return 0;

            // Lấy StatusId của Inactive
            var inactiveStatus = await _statusService.FirstOrDefaultAsync(s =>
                s.Name == StatusVariables.Inactive && s.EntityType == EntityVariables.Discount)
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var discountsToUpdate = new List<Discount>();

            foreach (var discount in activeDiscounts)
            {
                bool needUpdate = false;

                if ((discount.UsageLimit.HasValue && discount.UsedCount >= discount.UsageLimit.Value) ||
                    (discount.EndAt.HasValue && discount.EndAt.Value.Date < DateTime.UtcNow.Date))
                {
                    discount.StatusId = inactiveStatus.StatusId;
                    discount.UpdatedAt = DateTime.UtcNow;
                    needUpdate = true;
                }

                if (needUpdate)
                    discountsToUpdate.Add(discount);
            }

            if (discountsToUpdate.Any())
                await _discountService.UpdateRangeAsync(discountsToUpdate);

            return discountsToUpdate.Count;
        }
    }
}