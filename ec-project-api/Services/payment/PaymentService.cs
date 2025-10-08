using ec_project_api.Interfaces;
using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.payment
{
    public class PaymentService : BaseService<PaymentMethod, int>
    {
        public PaymentService(IRepository<PaymentMethod, int> repository) : base(repository)
        {
        }
    }
}
