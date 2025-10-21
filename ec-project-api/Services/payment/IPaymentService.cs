using ec_project_api.Models;
using ec_project_api.Services.Bases;
using static ec_project_api.Dtos.request.payments.SepayCreatePaymentRequest;

namespace ec_project_api.Services.Interfaces
{
    public interface IPaymentService : IBaseService<Payment, int>
    {   /// <summary>
        /// Tạo chuỗi URL QR Code của Sepay
        /// </summary>
        string CreateQrCodeUrl(CreateQRRequest request);
    }
}
