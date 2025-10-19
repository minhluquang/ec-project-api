using ec_project_api.Models;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.Interfaces
{
    public interface IPaymentDestinationService : IBaseService<PaymentDestination, int>
    {
        Task<bool> UpdateStatusAsync(int id, short newStatusId);
        Task<bool> UpdateBankInfoAsync(int id, string bankName, string accountName, string identifier);
        Task<PaymentDestination> GetActiveDestinationByActiveStatusId(short activeStatusDestinationId);
    }
}
