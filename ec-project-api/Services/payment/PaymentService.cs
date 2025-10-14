using ec_project_api.Models;
using Microsoft.EntityFrameworkCore;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using ec_project_api.Interfaces.Payments;
using ec_project_api.Services.Interfaces;

namespace ec_project_api.Services.payment
{
    public class PaymentService : BaseService<Payment, int>, IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
            : base(paymentRepository)
        {
            _paymentRepository = paymentRepository;
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
    }
}
