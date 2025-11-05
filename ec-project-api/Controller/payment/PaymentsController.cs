using ec_project_api.Constants.variables;
using ec_project_api.Dtos.response; // Dùng cho ResponseData<T>
using ec_project_api.Facades.Payments; // Dùng Facade
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using static ec_project_api.Dtos.request.payments.SepayCreatePaymentRequest;

namespace ec_project_api.Controllers.payments
{
    [ApiController]
    [Route(PathVariables.PaymentRoot)]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentFacade _paymentFacade;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            PaymentFacade paymentFacade,
            ILogger<PaymentController> logger)
        {
            _paymentFacade = paymentFacade;
            _logger = logger;
        }

        [HttpPost("create-payment")]
        public async Task<ActionResult<ResponseData<PaymentResponse>>> CreatePayment([FromBody] CreatePaymentRequest request)
        {

            try
            {
                var result = await _paymentFacade.CreatePaymentAsync(request);
                return Ok(ResponseData<PaymentResponse>.Success(StatusCodes.Status200OK, result));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Không tìm thấy: {ex.Message}");
                return NotFound(ResponseData<string>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, $"Lỗi tham số: {ex.Message}");
                return BadRequest(ResponseData<string>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
            catch (InvalidOperationException ex) // Lỗi cấu hình/nghiệp vụ
            {
                _logger.LogError(ex, $"Lỗi nghiệp vụ/cấu hình: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseData<string>.Error(StatusCodes.Status500InternalServerError, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi tạo thanh toán.");
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseData<string>.Error(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi tạo thanh toán."));
            }
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> HandleWebhook()
        {
            try
            {
                string webhookData;
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    webhookData = await reader.ReadToEndAsync();
                }

                var authHeader = Request.Headers["Authorization"].ToString();

                var result = await _paymentFacade.HandleWebhookAsync(webhookData, authHeader);

                // Trả về kết quả trực tiếp từ Facade
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Xác thực webhook thất bại: {ex.Message}");
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex) // Lỗi cấu hình
            {
                _logger.LogError(ex, $"Lỗi cấu hình khi xử lý webhook: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi nghiêm trọng khi xử lý webhook.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Đã xảy ra lỗi máy chủ nội bộ." });
            }
        }

        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<ResponseData<object>>> GetOrderStatus(short orderId)
        {
            try
            {
                var result = await _paymentFacade.GetOrderStatusAsync(orderId);
                return Ok(ResponseData<object>.Success(StatusCodes.Status200OK, result));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning($"Không tìm thấy đơn hàng {orderId}: {ex.Message}");
                return NotFound(ResponseData<string>.Error(StatusCodes.Status404NotFound, ex.Message));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Mã đơn hàng không hợp lệ {orderId}: {ex.Message}");
                return BadRequest(ResponseData<string>.Error(StatusCodes.Status400BadRequest, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy trạng thái Order {orderId}");
                return StatusCode(StatusCodes.Status500InternalServerError, ResponseData<string>.Error(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi máy chủ nội bộ."));
            }
        }
    }
}