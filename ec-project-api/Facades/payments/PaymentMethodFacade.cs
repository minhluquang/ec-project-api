using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
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
            var statusDraft = await _statusService.FirstOrDefaultAsync(s => s.EntityType == EntityVariables.Order && s.Name == StatusVariables.Draft) ??
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var method = _mapper.Map<PaymentMethod>(request);
            method.StatusId = statusDraft.StatusId;

            var success = await _paymentMethodService.CreateAsync(method);
            if (!success)
                throw new InvalidOperationException(PaymentMethodMessages.PaymentMethodCreateFailed);

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
                    throw new InvalidOperationException(StatusMessages.StatusNotFound);
            }

            _mapper.Map(request, method);
            method.UpdatedAt = DateTime.UtcNow;

            var success = await _paymentMethodService.UpdateAsync(method);
            if (!success)
                throw new InvalidOperationException(PaymentMethodMessages.PaymentMethodUpdateFailed);

            return true;
        }

        /// <summary>
        /// Xóa phương thức thanh toán
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var method = await _paymentMethodService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(PaymentMethodMessages.PaymentMethodNotFound);

            if(method.Status.Name != StatusVariables.Draft)
                throw new InvalidOperationException(PaymentMethodMessages.PaymentMethodDeleteFailed);

            var success = await _paymentMethodService.DeleteAsync(method);
            if (!success)
                throw new InvalidOperationException(PaymentMethodMessages.PaymentMethodDeleteFailed);

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
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var success = await _paymentMethodService.UpdateStatusAsync(id, newStatusId);
            if (!success)
                throw new InvalidOperationException(PaymentMethodMessages.PaymentMethodUpdateFailed);

            return true;
        }
    }
}
