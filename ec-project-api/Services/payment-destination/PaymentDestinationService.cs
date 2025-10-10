using ec_project_api.Interfaces.Payments;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using ec_project_api.Services.Interfaces;

namespace ec_project_api.Services.payment
{
    public class PaymentDestinationService : BaseService<PaymentDestination, int>, IPaymentDestinationService
    {
        private readonly IPaymentDestinationRepository _paymentDestinationRepository;

        public PaymentDestinationService(IPaymentDestinationRepository paymentDestinationRepository)
            : base(paymentDestinationRepository)
        {
            _paymentDestinationRepository = paymentDestinationRepository;
        }

        // ✅ Lấy toàn bộ điểm đến thanh toán (kèm PaymentMethod và Status)
        public override async Task<IEnumerable<PaymentDestination>> GetAllAsync(QueryOptions<PaymentDestination>? options = null)
        {
            options ??= new QueryOptions<PaymentDestination>();

            // Include các navigation properties
            options.Includes.Add(d => d.PaymentMethod);
            options.Includes.Add(d => d.Status);

            // Nếu cần lọc theo trạng thái
            int? statusId = null;
            options.Filter = d => !statusId.HasValue || d.StatusId == statusId.Value;

            return await _paymentDestinationRepository.GetAllAsync(options);
        }

        // ✅ Lấy 1 điểm đến thanh toán theo ID (kèm PaymentMethod và Status)
        public override async Task<PaymentDestination?> GetByIdAsync(int id, QueryOptions<PaymentDestination>? options = null)
        {
            options ??= new QueryOptions<PaymentDestination>();
            options.Includes.Add(d => d.PaymentMethod);
            options.Includes.Add(d => d.Status);
            return await _paymentDestinationRepository.GetByIdAsync(id, options);
        }
        // ✅ Cập nhật trạng thái điểm đến thanh toán
        public async Task<bool> UpdateStatusAsync(int id, int newStatusId)
        {
            var destination = await _paymentDestinationRepository.GetByIdAsync(id);
            if (destination == null)
                return false;

            destination.StatusId = newStatusId;
            destination.UpdatedAt = DateTime.UtcNow;

            await _paymentDestinationRepository.UpdateAsync(destination);
            return await _paymentDestinationRepository.SaveChangesAsync() > 0;
        }

        // ✅ Cập nhật thông tin ngân hàng
        public async Task<bool> UpdateBankInfoAsync(int id, string bankName, string accountName, string imageUrl, string identifier)
        {
            var destination = await _paymentDestinationRepository.GetByIdAsync(id);
            if (destination == null)
                return false;

            destination.BankName = bankName;
            destination.AccountName = accountName;
            destination.ImageUrl = imageUrl;
            destination.Identifier = identifier;
            destination.UpdatedAt = DateTime.UtcNow;

            await _paymentDestinationRepository.UpdateAsync(destination);
            return await _paymentDestinationRepository.SaveChangesAsync() > 0;
        }

    }
}
