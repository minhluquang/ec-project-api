using ec_project_api.Dtos.response.payments;
using ec_project_api.Facades;
using ec_project_api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ec_project_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentFacade _paymentFacade;

        public PaymentController(PaymentFacade paymentFacade)
        {
            _paymentFacade = paymentFacade;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            var payment = await _paymentFacade.CreatePaymentAsync(dto);

            var response = new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                Amount = payment.Amount,
                TransactionId = payment.TransactionId,
                PaidAt = payment.PaidAt,
                StatusName = payment.StatusName,
                DestinationName = payment.DestinationName,
                OrderId = payment.OrderId,
            };
            return Ok(new
            {
                isSuccess = true,
                message = "Tạo thanh toán thành công",
                data = response
            });
        }
    }
}
