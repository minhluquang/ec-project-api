using ec_project_api.Interfaces.Payments;
using ec_project_api.Models;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(DataContext context) : base(context) { }
}
