using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.payment
{
    public interface IPaymentMethodService : IBaseService<PaymentMethod, int>
    {
        Task<bool> UpdateStatusAsync(int id, short newStatusId);
        Task<bool> UpdateMethodNameAsync(int id, string newName);
    }
}
