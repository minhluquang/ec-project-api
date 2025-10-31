using ec_project_api.Interfaces.Payments;
using ec_project_api.Models;

public class PaymentMethodRepository : Repository<PaymentMethod, int>, IPaymentMethodRepository
{
    public PaymentMethodRepository(DataContext context) : base(context) { }
}
