using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Interfaces;
using ec_project_api.Interfaces.Discounts;
using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ec_project_api.Services.discounts
{
    public class DiscountService : BaseService<Discount, int>, IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountService(IDiscountRepository discountRepository) : base(discountRepository)
        {
            _discountRepository = discountRepository;
        }
        public async Task<bool> CheckAndUpdateDiscountStatusByIdAsync(int discountId, short inactiveStatusId)
        {
            var discount = await GetByIdAsync(discountId);
            if (discount == null) return false;

            bool updated = false;

            if ((discount.UsageLimit.HasValue && discount.UsedCount >= discount.UsageLimit.Value) ||
                (discount.EndAt.HasValue && discount.EndAt.Value.Date < DateTime.UtcNow.Date))
            {
                discount.StatusId = inactiveStatusId;
                discount.UpdatedAt = DateTime.UtcNow;
                await UpdateAsync(discount); // gọi BaseService.UpdateAsync
                updated = true;
            }

            return updated;
        }



    }
}