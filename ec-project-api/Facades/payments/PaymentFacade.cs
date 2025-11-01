using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Services.Interfaces;
using ec_project_api.Services.orders;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using static ec_project_api.Dtos.request.payments.SepayCreatePaymentRequest;

namespace ec_project_api.Facades.Payments
{
    public class PaymentFacade
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IPaymentDestinationService _paymentDestinationService;
        private readonly ILogger<PaymentFacade> _logger;
        private readonly IConfiguration _configuration;
        private readonly IStatusService _statusService;

        public PaymentFacade(
            IPaymentService paymentService,
            IOrderService orderService,
            IPaymentDestinationService paymentDestinationService,
            ILogger<PaymentFacade> logger,
            IConfiguration configuration,
            IStatusService statusService
            )
        {
            _paymentService = paymentService;
            _orderService = orderService;
            _paymentDestinationService = paymentDestinationService;
            _logger = logger;
            _configuration = configuration;
            _statusService = statusService;
        }

        public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
        {
            var activeStatus = await _statusService.FirstOrDefaultAsync(s => s.EntityType == EntityVariables.PaymentDestination && s.Name == StatusVariables.Active) ??
               throw new InvalidOperationException(StatusMessages.StatusNotFound);
            // 1. Lấy tài khoản ngân hàng active
            var activeDestination = await _paymentDestinationService.GetActiveDestinationByActiveStatusId(activeStatus.StatusId);
            if (activeDestination == null)
            {
                _logger.LogWarning($"Không tìm thấy tài khoản nhận tiền active (StatusId = {activeStatus.StatusId})");
                // Ném lỗi để Controller bắt
                throw new InvalidOperationException("Lỗi cấu hình máy chủ: Không tìm thấy tài khoản nhận tiền.");
            }


            var order = await _orderService.GetByIdAsync(request.OrderId);
            if (order == null)
            {
                _logger.LogWarning($"Không tìm thấy Order/Payment cho ID: {request.OrderId}");
                throw new KeyNotFoundException("Không tìm thấy đơn hàng.");
            }
            var payment = order.Payment ?? new Payment();

            order.Payment = payment;

            var PAYMENT_PAID_STATUS = await _statusService.FirstOrDefaultAsync(s => s.EntityType == EntityVariables.Payment && s.Name == StatusVariables.Completed) ??
               throw new InvalidOperationException(StatusMessages.StatusNotFound);

            // 3. Kiểm tra dùng lại QR cũ
            if (payment.StatusId != PAYMENT_PAID_STATUS.StatusId && !string.IsNullOrEmpty(payment.QrCodeUrl))
            {
                _logger.LogInformation($"Sử dụng lại QR code cho order: {request.OrderId}");
                return new PaymentResponse
                {
                    Success = true,
                    Message = "Existing payment QR code reused.",
                    QrCodeUrl = payment.QrCodeUrl,
                    TransactionId = payment.TransactionId
                };
            }
            var qrRequest = new CreateQRRequest
            {
                OrderId = request.OrderId,
                Amount = request.Amount,
                Description = request.Description
            };

            // 4. Tạo QR Code URL
            _logger.LogInformation($"Tạo QR code mới cho order: {request.OrderId}");
            qrRequest.BankAccountNumber = activeDestination.Identifier;
            qrRequest.BankCode = activeDestination.BankName;
            qrRequest.Description = request.Description; // Nội dung thanh toán CHỈ LÀ mã đơn hàng

            string qrCodeUrl = _paymentService.CreateQrCodeUrl(qrRequest);

            var PAYMENT_PENDING_STATUS = await _statusService.FirstOrDefaultAsync(s => s.EntityType == EntityVariables.Payment && s.Name == StatusVariables.Pending) ??
               throw new InvalidOperationException(StatusMessages.StatusNotFound);

            // 5. Cập nhật bảng Payment thật
            payment.QrCodeUrl = qrCodeUrl;
            payment.Amount = request.Amount;
            payment.Description = request.Description;
            payment.StatusId = PAYMENT_PENDING_STATUS.StatusId;
            payment.UpdatedAt = DateTime.UtcNow;

            await _paymentService.CreateAsync(payment);
            await _paymentService.SaveChangesAsync();

            return new PaymentResponse
            {
                Success = true,
                Message = "Payment QR code created successfully",
                QrCodeUrl = qrCodeUrl,
                TransactionId = payment.TransactionId
            };
        }

        public async Task<object> HandleWebhookAsync(string webhookData, string authHeader)
        {
            _logger.LogInformation("Sepay SecretApiKey chưa được cấu hình.");

            var PAYMENT_PENDING_STATUS = await _statusService.FirstOrDefaultAsync(s => s.EntityType == EntityVariables.Payment && s.Name == StatusVariables.Pending) ??
               throw new InvalidOperationException(StatusMessages.StatusNotFound);
            var PAYMENT_PAID_STATUS = await _statusService.FirstOrDefaultAsync(s => s.EntityType == EntityVariables.Payment && s.Name == StatusVariables.Completed) ??
              throw new InvalidOperationException(StatusMessages.StatusNotFound);
            // 1. Xác thực API Key
            var secretApiKey = _configuration["Sepay:SecretApiKey"];
            if (string.IsNullOrEmpty(secretApiKey))
            {
                _logger.LogError("Sepay SecretApiKey chưa được cấu hình.");
                throw new InvalidOperationException("Lỗi cấu hình máy chủ.");
            }
            if (string.IsNullOrEmpty(authHeader) ||
                authHeader.Replace("Apikey ", "").Trim() != secretApiKey)
            {
                _logger.LogWarning("Webhook với API Key không hợp lệ.");
                _logger.LogWarning($"secretApiKey: {secretApiKey}.");
                _logger.LogWarning($"authHeader: {authHeader}.");
                throw new UnauthorizedAccessException("API Key không hợp lệ");
            }

            // 2. Parse Webhook
            _logger.LogInformation($"Webhook received: {webhookData}");
            var webhook = JsonConvert.DeserializeObject<SepayWebhookModel>(webhookData);

            if (webhook.TransferType != "in")
            {
                _logger.LogInformation($"Bỏ qua webhook (không phải tiền vào): {webhook.Id}");
                return new { success = true, message = "Webhook skipped." };
            }

            // 3. Lấy OrderId và tìm Order/Payment
            string orderIdString = webhook.Content;

            int? orderIdInt = ExtractOrderId(webhook.Content);
            if (orderIdInt == null)
            {
                _logger.LogWarning($"Không tìm thấy mã đơn hàng hợp lệ trong nội dung: {webhook.Content}");
                return new { success = true, message = "Invalid order id format." };
            }
            var order = await _orderService.GetByIdAsync((int)orderIdInt);
            if (order == null || order.Payment == null)
            {
                _logger.LogWarning($"Không tìm thấy Order/Payment thật cho ID: {orderIdInt}");
                return new { success = true, message = "Order not found." };
            }
            var payment = order.Payment;

            // 4. Kiểm tra logic (Amount, Status)
            if (payment.Amount != webhook.TransferAmount)
            {
                _logger.LogWarning($"SAI SỐ TIỀN cho OrderId: {orderIdString}. Mong đợi: {payment.Amount}, Nhận được: {webhook.TransferAmount}");
                return new { success = true, message = "Amount mismatch." };
            }

            var ORDER_PROCESSING_STATUS = await _statusService.FirstOrDefaultAsync(s => s.EntityType == EntityVariables.Order && s.Name == StatusVariables.Processing) ??
              throw new InvalidOperationException(StatusMessages.StatusNotFound);
            // 5. Cập nhật
            if (payment.StatusId != PAYMENT_PAID_STATUS.StatusId)
            {
                _logger.LogInformation($"Xác nhận thanh toán cho OrderId: {orderIdString}");

                payment.StatusId = PAYMENT_PAID_STATUS.StatusId;
                payment.PaidAt = webhook.TransactionDate;
                payment.TransactionId = webhook.Id.ToString();
                payment.SepayTransactionId = webhook.Id.ToString();
                payment.SepayResponse = webhookData;
                payment.UpdatedAt = DateTime.UtcNow;

                await _paymentService.UpdateAsync(payment);
                await _orderService.UpdateOrderStatusAsync(order.OrderId, ORDER_PROCESSING_STATUS.StatusId);

                _logger.LogInformation($"Cập nhật thành công Order {order.OrderId} và Payment {payment.PaymentId}");
            }
            else
            {
                _logger.LogInformation($"Webhook cho OrderId {orderIdString} đã được xử lý (Status: {payment.StatusId}).");
            }

            return new { success = true, message = "Webhook processed successfully" };
        }

        public async Task<object> GetOrderStatusAsync(int orderId)
        {

            // Dùng GetByIdAsync của OrderService, nó đã bao gồm "Status"
            var order = await _orderService.GetByIdAsync(orderId);
            if (order == null || order.Payment == null || order.Status == null)
            {
                throw new KeyNotFoundException("Không tìm thấy đơn hàng hoặc thông tin thanh toán.");
            }

            var payment = order.Payment;
            var PAYMENT_PAID_STATUS = await _statusService.FirstOrDefaultAsync(s => s.EntityType == EntityVariables.Payment && s.Name == StatusVariables.Completed) ??
              throw new InvalidOperationException(StatusMessages.StatusNotFound);
            // Trả về một đối tượng DTO (thay vì anonymous type) sẽ tốt hơn, 
            // nhưng ở đây tôi dùng anonymous type cho nhanh
            return new
            {
                success = true,
                orderId = orderId,
                status = order.Status.Name, // Đảm bảo order.Status đã được load
                amount = payment.Amount,
                qrCodeUrl = payment.QrCodeUrl,
                isPaid = (payment.StatusId == PAYMENT_PAID_STATUS.StatusId),
                paidAt = payment.PaidAt
            };
        }

        /// <summary>
        /// Trích xuất mã đơn hàng (số nguyên) từ nội dung giao dịch có prefix "ORD".
        /// Ví dụ: "MBVCB.11534558217.298990.ORD34.CT ..." => 34
        /// </summary>
        public static int? ExtractOrderId(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return null;

            // Tìm pattern "ORD" + dãy số
            var match = Regex.Match(content, @"ORD(\d+)", RegexOptions.IgnoreCase);
            if (!match.Success)
                return null;

            if (int.TryParse(match.Groups[1].Value, out int orderId))
                return orderId;

            return null;
        }
    }
}