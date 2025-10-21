using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.DTOs.Payments;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Services.Interfaces;
using ec_project_api.Services.payment;

namespace ec_project_api.Facades.payments
{
    public class PaymentDestinationFacade
    {
        private readonly IPaymentDestinationService _paymentDestinationService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public PaymentDestinationFacade(
            IPaymentDestinationService paymentDestinationService,
            IPaymentMethodService paymentMethodService,
            IStatusService statusService,
            IMapper mapper)
        {
            _paymentDestinationService = paymentDestinationService;
            _paymentMethodService = paymentMethodService;
            _statusService = statusService;
            _mapper = mapper;
        }

        // ✅ Lấy tất cả PaymentDestination (kèm PaymentMethod và Status)
        public async Task<IEnumerable<PaymentDestinationDto>> GetAllAsync()
        {
            var destinations = await _paymentDestinationService.GetAllAsync();
            return _mapper.Map<IEnumerable<PaymentDestinationDto>>(destinations);
        }

        // ✅ Lấy theo ID
        public async Task<PaymentDestinationDto> GetByIdAsync(int id)
        {
            var destination = await _paymentDestinationService.GetByIdAsync(id);
            if (destination == null)
                throw new KeyNotFoundException(PaymentDestinationMessages.PaymentDestinationNotFound);

            return _mapper.Map<PaymentDestinationDto>(destination);
        }

        // ✅ Tạo mới
        public async Task<bool> CreateAsync(PaymentDestinationCreateRequest request)
        {
            var method = await _paymentMethodService.GetByIdAsync(request.PaymentMethodId);
            if (method == null)
                throw new InvalidOperationException(PaymentMethodMessages.PaymentMethodNotFound);

            var status = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.PaymentDestination && s.Name == StatusVariables.Draft);
            if (status == null)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var destination = _mapper.Map<PaymentDestination>(request);
            destination.StatusId = status.StatusId;

            return await _paymentDestinationService.CreateAsync(destination);
        }
        public async Task<bool> UpdateAsync(int id, PaymentDestinationUpdateRequest request)
        {
            var existing = await _paymentDestinationService.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException(PaymentDestinationMessages.PaymentDestinationNotFound);

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            var result = await _paymentDestinationService.UpdateAsync(existing);
            if (!result)
                throw new InvalidOperationException(PaymentDestinationMessages.PaymentDestinationUpdateFailed);

            return result;
        }

        // ✅ Cập nhật thông tin ngân hàng
        public async Task<bool> UpdateBankInfoAsync(int id, PaymentDestinationUpdateRequest request)
        {
            var destination = await _paymentDestinationService.GetByIdAsync(id);
            if (destination == null)
                throw new KeyNotFoundException(PaymentDestinationMessages.PaymentDestinationNotFound);

            return await _paymentDestinationService.UpdateBankInfoAsync(
                id,
                request.BankName,
                request.AccountName,
                request.Identifier
            );
        }

        // ✅ Cập nhật trạng thái
        public async Task<bool> UpdateStatusAsync(int id, short newStatusId)
        {
            var status = await _statusService.GetByIdAsync(newStatusId);
            if (status == null || status.EntityType != EntityVariables.PaymentDestination)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var result = await _paymentDestinationService.UpdateStatusAsync(id, newStatusId);
            if (!result)
                throw new InvalidOperationException(PaymentDestinationMessages.PaymentDestinationUpdateFailed);

            return true;
        }

        // ✅ Xóa
        public async Task<bool> DeleteAsync(int id)
        {
            var destination = await _paymentDestinationService.GetByIdAsync(id);
            if (destination == null)
                throw new KeyNotFoundException(PaymentDestinationMessages.PaymentDestinationNotFound);
                                  
            if (destination.Status.Name != StatusVariables.Draft)
                throw new InvalidOperationException(PaymentDestinationMessages.PaymentDestinationDeleteFailed);

            return await _paymentDestinationService.DeleteByIdAsync(id);

        }
    }
}
