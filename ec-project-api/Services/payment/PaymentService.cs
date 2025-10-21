using ec_project_api.Interfaces.Payments;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using ec_project_api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Web;
using static ec_project_api.Dtos.request.payments.SepayCreatePaymentRequest;

namespace ec_project_api.Services.payment
{
    public class PaymentService : BaseService<Payment, int>, IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentService> _logger; // Thêm logger
        public PaymentService(
             IPaymentRepository paymentRepository,
             IConfiguration configuration,
             ILogger<PaymentService> logger) // Inject ILogger
             : base(paymentRepository)
        {
            _paymentRepository = paymentRepository;
            _configuration = configuration;
            _logger = logger;
        }


        public override async Task<IEnumerable<Payment>> GetAllAsync(QueryOptions<Payment>? options = null)
        {
            options ??= new QueryOptions<Payment>();

            options.Includes.Add(p => p.PaymentDestination!);
            options.IncludeThen.Add(q => q
                .Include(p => p.PaymentDestination!)
                    .ThenInclude(d => d.PaymentMethod));
            options.Includes.Add(p => p.Status);
            options.Includes.Add(p => p.Order!);

            return await _paymentRepository.GetAllAsync(options);
        }

        public override async Task<Payment?> GetByIdAsync(int id, QueryOptions<Payment>? options = null)
        {
            options ??= new QueryOptions<Payment>();

            options.Includes.Add(p => p.PaymentDestination!);
            options.IncludeThen.Add(q => q
                .Include(p => p.PaymentDestination!)
                    .ThenInclude(d => d.PaymentMethod));
            options.Includes.Add(p => p.Status);
            options.Includes.Add(p => p.Order!);

            return await _paymentRepository.GetByIdAsync(id, options);
        }
        public string CreateQrCodeUrl(CreateQRRequest request)
        {

                string baseUrl = _configuration["Sepay:QR_BASE_URL"];

             
                var queryParams = new StringBuilder();

                queryParams.Append($"bank={HttpUtility.UrlEncode(request.BankCode)}");
                queryParams.Append($"&acc={HttpUtility.UrlEncode(request.BankAccountNumber)}");
                queryParams.Append($"&amount={request.Amount}");

                queryParams.Append($"&des={HttpUtility.UrlEncode(request.Description)}");
                queryParams.Append("&template=compact"); 

                string qrCodeUrl = $"{baseUrl}?{queryParams}";

                _logger.LogInformation($"Tạo QR Code URL: {qrCodeUrl}");
                return qrCodeUrl;

        }
    }

}
