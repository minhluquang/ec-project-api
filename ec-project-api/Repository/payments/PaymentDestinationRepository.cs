using ec_project_api.Interfaces.Payments;
using ec_project_api.Models;

public class PaymentDestinationRepository : Repository<PaymentDestination>, IPaymentDestinationRepository
{
    public PaymentDestinationRepository(DataContext context) : base(context) { }
}
