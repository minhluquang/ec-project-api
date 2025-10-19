using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.orders {
    public interface IOrderService : IBaseService<Order, int> {
        Task<Order> CreateOrderAsync(Order order);
        Task<bool> UpdateOrderStatusAsync(int orderId, short newStatusId);
        /// <summary>
        /// Xác nhận thanh toán từ Sepay, cập nhật cả Order và Payment.
        /// </summary>
        /// <param name="orderIdString">Mã đơn hàng (dạng string, ví dụ "DH12345")</param>
        /// <param name="sepayTransactionId">Mã giao dịch từ Sepay (ví dụ: "92704")</param>
        /// <param name="paidAt">Thời gian thanh toán từ Sepay</param>
        /// <param name="sepayResponseData">Toàn bộ JSON thô từ webhook Sepay</param>
        /// <returns>True nếu cập nhật thành công</returns>
        Task<bool> ConfirmSepayPaymentAsync(string orderIdString, string sepayTransactionId, DateTime paidAt, string sepayResponseData);
    }
}                                                                       