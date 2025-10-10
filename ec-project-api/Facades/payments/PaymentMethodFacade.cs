using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Dtos.request.payments;
using ec_project_api.Dtos.response.payments;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Services.payment;

namespace ec_project_api.Facades.PaymentMethods
{
    public class PaymentMethodFacade
    {
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public PaymentMethodFacade(
            IPaymentMethodService paymentMethodService,
            IStatusService statusService,
            IMapper mapper)
        {
            _paymentMethodService = paymentMethodService;
            _statusService = statusService;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy tất cả phương thức thanh toán
        /// </summary>
        public async Task<IEnumerable<PaymentMethodDto>> GetAllAsync()
        {
            var methods = await _paymentMethodService.GetAllAsync();
            return _mapper.Map<IEnumerable<PaymentMethodDto>>(methods);
        }

        /// <summary>
        /// Lấy phương thức thanh toán theo ID
        /// </summary>
        public async Task<PaymentMethodDto> GetByIdAsync(int id)
        {
            var method = await _paymentMethodService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(PaymentMethodMessages.PaymentMethodNotFound);

            return _mapper.Map<PaymentMethodDto>(method);
        }

        /// <summary>
        /// Tạo mới phương thức thanh toán
        /// </summary>
        public async Task<PaymentMethodDto> CreateAsync(PaymentMethodCreateRequest request)
        {
            var activeStatus = await _statusService.GetByIdAsync(22);
            if (activeStatus == null)
                throw new InvalidOperationException("Không tìm thấy trạng thái hợp lệ.");

            var method = _mapper.Map<PaymentMethod>(request);
            method.StatusId = activeStatus.StatusId;

            var success = await _paymentMethodService.CreateAsync(method);
            if (!success)
                throw new InvalidOperationException("Không thể tạo phương thức thanh toán.");

            return _mapper.Map<PaymentMethodDto>(method);
        }

        /// <summary>
        /// Cập nhật phương thức thanh toán
        /// </summary>
        public async Task<bool> UpdateAsync(int id, PaymentMethodUpdateRequest request)
        {
            var method = await _paymentMethodService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(PaymentMethodMessages.PaymentMethodNotFound);

            if (request.StatusId != 0)
            {
                var status = await _statusService.GetByIdAsync(request.StatusId);
                if (status == null)
                    throw new InvalidOperationException("Trạng thái không hợp lệ.");
            }

            _mapper.Map(request, method);
            method.UpdatedAt = DateTime.UtcNow;

            var success = await _paymentMethodService.UpdateAsync(method);
            if (!success)
                throw new InvalidOperationException("Không thể cập nhật phương thức thanh toán.");

            return true;
        }

        /// <summary>
        /// Xóa phương thức thanh toán
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var method = await _paymentMethodService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(PaymentMethodMessages.PaymentMethodNotFound);

            var success = await _paymentMethodService.DeleteAsync(method);
            if (!success)
                throw new InvalidOperationException("Không thể xóa phương thức thanh toán.");

            return true;
        }

        /// <summary>
        /// Cập nhật trạng thái phương thức thanh toán
        /// </summary>
        public async Task<bool> UpdateStatusAsync(int id, int newStatusId)
        {
            var method = await _paymentMethodService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(PaymentMethodMessages.PaymentMethodNotFound);

            var status = await _statusService.GetByIdAsync(newStatusId)
                ?? throw new InvalidOperationException("Trạng thái không hợp lệ.");

            var success = await _paymentMethodService.UpdateStatusAsync(id, newStatusId);
            if (!success)
                throw new InvalidOperationException("Không thể cập nhật trạng thái.");

            return true;
        }
    }
}
