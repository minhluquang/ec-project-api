using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Interfaces.Orders;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Thêm logger

namespace ec_project_api.Services.orders
{
    public class OrderService : BaseService<Order, int>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderService> _logger; // Thêm logger

        public OrderService(IOrderRepository orderRepository)
            : base(orderRepository)
        {
            _orderRepository = orderRepository;
          
        }

        // ✅ Lấy tất cả đơn hàng (kèm User, Status, OrderItems)
        public override async Task<IEnumerable<Order>> GetAllAsync(QueryOptions<Order>? options = null)
        {
            options ??= new QueryOptions<Order>();

            // Include các quan hệ chính
            options.Includes.Add(o => o.User);
            options.Includes.Add(o => o.Status);
            options.Includes.Add(o => o.Discount);
            options.Includes.Add(o => o.Ship);
            options.Includes.Add(o => o.Payment);
            options.Includes.Add(o => o.OrderItems);

            // ✅ Include sâu các mối quan hệ của OrderItems
            options.IncludeThen.Add(q => q
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Product));

            // ✅ Thêm Include cho Size (cấp sâu hơn)
            options.IncludeThen.Add(q => q
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Size));
            var orders = await _orderRepository.GetAllAsync(options);
            return orders;
        }

        // ✅ Lấy đơn hàng theo ID (kèm các thông tin liên quan)
        public override async Task<Order?> GetByIdAsync(int id, QueryOptions<Order>? options = null)
        {
            options ??= new QueryOptions<Order>();

            options.Includes.Add(o => o.User);
            options.Includes.Add(o => o.Status);
            options.Includes.Add(o => o.Discount);
            options.Includes.Add(o => o.Ship);
            options.Includes.Add(o => o.Payment);
            options.Includes.Add(o => o.OrderItems);

            // ✅ Dùng IncludeThen cho các cấp sâu
            options.IncludeThen.Add(q => q
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Product));

            return await _orderRepository.GetByIdAsync(id, options);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();
            return order;
        }
        public async Task<bool> UpdateOrderStatusAsync(int orderId, short newStatusId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new InvalidOperationException(OrderMessages.OrderNotFound);

            order.StatusId = newStatusId;
            order.UpdatedAt = DateTime.UtcNow;

            if (order.Status?.Name == "Shipped")
                order.ShippedAt = DateTime.UtcNow;

            if (order.Status?.Name == "Delivered")
                order.DeliveryAt = DateTime.UtcNow;

            return await UpdateAsync(order);
        }

        public async Task<bool> ConfirmSepayPaymentAsync(string orderIdString, string sepayTransactionId, DateTime paidAt, string sepayResponseData)
        {
            // ---- BƯỚC 1: ĐỊNH NGHĨA STATUS IDS CỦA BẠN ----
            // RẤT QUAN TRỌNG: Hãy thay thế các con số này bằng StatusId THẬT trong database của bạn
            const short ORDER_STATUS_PROCESSING = 19; // Ví dụ: 2 = "Đang xử lý"
            const short PAYMENT_STATUS_PAID = 32;    // Ví dụ: 10 = "Đã thanh toán"
            // ------------------------------------------------

            _logger.LogInformation($"Bắt đầu xác nhận thanh toán Sepay cho OrderId: {orderIdString}");

            // BƯỚC 2: CHUYỂN ĐỔI VÀ TÌM ĐƠN HÀNG
            // Giả định: `orderIdString` (ví dụ: "DH12345") là Order.Order_id (int) được lưu dưới dạng string.
            // Nếu "DH12345" là một mã code riêng, bạn cần thay GetByIdAsync bằng một hàm tìm kiếm khác (ví dụ: GetByCodeAsync).
            // Ở đây tôi giả định nó là PK.
            if (!int.TryParse(orderIdString.Replace("DH", ""), out int orderId))
            {
                _logger.LogError($"Không thể parse OrderId: {orderIdString} thành số nguyên.");
                return false;
            }

            var order = await GetByIdAsync(orderId); // Dùng hàm GetByIdAsync đã có sẵn

            if (order == null)
            {
                _logger.LogWarning($"Không tìm thấy Order với ID: {orderId} (từ chuỗi {orderIdString})");
                return false;
            }

            // BƯỚC 3: KIỂM TRA VÀ CẬP NHẬT
            if (order.Payment == null)
            {
                _logger.LogError($"Đơn hàng {orderId} không có thông tin Payment liên kết.");
                return false;
            }

            try
            {
                // Cập nhật bảng Payment
                order.Payment.StatusId = PAYMENT_STATUS_PAID;
                order.Payment.TransactionId = sepayTransactionId; // Cập nhật mã giao dịch của Sepay
                order.Payment.PaidAt = paidAt;
                order.Payment.SepayResponse = sepayResponseData; // Lưu lại toàn bộ response
                order.Payment.UpdatedAt = DateTime.UtcNow;

                // Cập nhật bảng Order
                order.StatusId = ORDER_STATUS_PROCESSING;
                order.UpdatedAt = DateTime.UtcNow;

                // BƯỚC 4: LƯU THAY ĐỔI
                // Do `GetByIdAsync` đã include `Payment`, EF Core sẽ theo dõi thay đổi của cả hai
                await _orderRepository.SaveChangesAsync();

                _logger.LogInformation($"Cập nhật Order {orderId} và Payment {order.Payment.PaymentId} thành công.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lưu cập nhật cho OrderId: {orderId}");
                return false;
            }
        }
    }
}

