using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Interfaces.Payments
{
    public interface IPaymentDestinationService : IBaseService<PaymentDestination, int>
    {
        Task<IEnumerable<PaymentDestination>> GetAllAsync(int? pageNumber = 1, int? pageSize = 10, int? statusId = null, string? identifier = null, string? orderBy = null);
        new Task<ec_project_api.Dtos.response.pagination.PagedResult<PaymentDestination>> GetAllPagedAsync(QueryOptions<PaymentDestination> options);
        Task<bool> UpdateStatusAsync(int id, short newStatusId);
        Task<bool> DeleteAsync(PaymentDestination entity, short newStatusId);
    }
}
