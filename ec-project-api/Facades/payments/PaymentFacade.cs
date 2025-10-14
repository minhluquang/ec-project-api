using AutoMapper;
using ec_project_api.Dtos.response.payments;
using ec_project_api.Models;
using ec_project_api.Services.Interfaces;
using ec_project_api.Services.orders;
using ec_project_api.Services.payment;

namespace ec_project_api.Facades
{
    public class PaymentFacade 
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IPaymentDestinationService _paymentDestinationService;
        private readonly IMapper _mapper;

        public PaymentFacade(
            IPaymentService paymentService,
            IOrderService orderService      ,
            IPaymentDestinationService paymentDestinationService,
            IMapper mapper
        )
        {
            _paymentService = paymentService;
            _orderService = orderService;    
            _paymentDestinationService = paymentDestinationService;
            _mapper = mapper;
        }

        public async Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentDto dto)
        {

            var destination = await _paymentDestinationService.FirstOrDefaultAsync(d => d.DestinationId == dto.DestinationId);
            // map từ DTO sang entity
            var payment = _mapper.Map<Payment>(dto);

            await _paymentService.CreateAsync(payment);
            await _paymentService.SaveChangesAsync(); // Lưu thay đổi
            // Nếu có OrderId, cập nhật order để gắn PaymentId
            if (dto.OrderId.HasValue)
            {
                var order = await _orderService.GetByIdAsync(dto.OrderId.Value);
                if (order != null)
                {
                    order.PaymentId = payment.PaymentId;
                    await _orderService.UpdateAsync(order);
                    await _orderService.SaveChangesAsync();
                }
            }
             var result = _mapper.Map<PaymentResponseDto>(payment);
             result.OrderId = dto.OrderId;
             result.DestinationName = destination?.BankName;

    return result;
        }
    }
}
