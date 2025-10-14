using ec_project_api.Interfaces;
using ec_project_api.Interfaces.Payments;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.payment
{
    public class PaymentMethodService : BaseService<PaymentMethod, int>  , IPaymentMethodService
    {
        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository) : base(paymentMethodRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
        }
        // Lấy toàn bộ phương thức thanh toán (kèm Status và PaymentDestinations)
        public override async Task<IEnumerable<PaymentMethod>> GetAllAsync(QueryOptions<PaymentMethod>? options = null)
        {
            options ??= new QueryOptions<PaymentMethod>();

            // include các navigation properties
            options.Includes.Add(p => p.Status);
            options.Includes.Add(p => p.PaymentDestinations);

            // nếu cần lọc theo trạng thái
            int? statusId = null;
            options.Filter = p => !statusId.HasValue || p.StatusId == statusId.Value;

            return await _paymentMethodRepository.GetAllAsync(options);
        }

        // Lấy 1 phương thức thanh toán theo ID (kèm Status và PaymentDestinations)
        public override async Task<PaymentMethod?> GetByIdAsync(int id, QueryOptions<PaymentMethod>? options = null)
        {
            options ??= new QueryOptions<PaymentMethod>();
            options.Includes.Add(p => p.Status);
            options.Includes.Add(p => p.PaymentDestinations);
            return await _paymentMethodRepository.GetByIdAsync(id, options);
        }

        // Cập nhật trạng thái của PaymentMethod
        public async Task<bool> UpdateStatusAsync(int id, int newStatusId)
        {
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id);
            if (paymentMethod == null)
                return false;

            paymentMethod.StatusId = newStatusId;
            paymentMethod.UpdatedAt = DateTime.UtcNow;

            await _paymentMethodRepository.UpdateAsync(paymentMethod);
            return await _paymentMethodRepository.SaveChangesAsync() > 0;
        }

        // Cập nhật tên phương thức thanh toán
        public async Task<bool> UpdateMethodNameAsync(int id, string newName)
        {
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id);
            if (paymentMethod == null)
                return false;

            paymentMethod.MethodName = newName;
            paymentMethod.UpdatedAt = DateTime.UtcNow;

            await _paymentMethodRepository.UpdateAsync(paymentMethod);
            return await _paymentMethodRepository.SaveChangesAsync() > 0;
        }
    }
}
