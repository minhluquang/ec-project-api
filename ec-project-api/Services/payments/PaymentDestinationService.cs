using ec_project_api.Constants.variables;
using ec_project_api.Interfaces.Payments;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services.payments
{
    public class PaymentDestinationService : BaseService<PaymentDestination, int>, IPaymentDestinationService
    {
        private readonly IPaymentDestinationRepository _repo;

        public PaymentDestinationService(IPaymentDestinationRepository repo) : base(repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<PaymentDestination>> GetAllAsync(int? pageNumber = 1, int? pageSize = 10, int? statusId = null, string? identifier = null, string? orderBy = null)
        {
            var options = new QueryOptions<PaymentDestination>
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            options.Filter = pd =>
                (!statusId.HasValue || pd.StatusId == statusId.Value) &&
                (string.IsNullOrEmpty(identifier) || pd.Identifier.Contains(identifier));

            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy)
                {
                    case "id_desc":
                        options.OrderBy = q => q.OrderByDescending(p => p.DestinationId);
                        break;
                    case "id_asc":
                        options.OrderBy = q => q.OrderBy(p => p.DestinationId);
                        break;
                }
            }

            return await base.GetAllAsync(options);
        }

        public override async Task<ec_project_api.Dtos.response.pagination.PagedResult<PaymentDestination>> GetAllPagedAsync(QueryOptions<PaymentDestination>? options = null)
        {
            return await base.GetAllPagedAsync(options);
        }

        public override async Task<PaymentDestination?> GetByIdAsync(int id, QueryOptions<PaymentDestination>? options = null)
        {
            options ??= new QueryOptions<PaymentDestination>();
            return await base.GetByIdAsync(id, options);
        }

        public async Task<bool> UpdateStatusAsync(int id, short newStatusId)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            existing.StatusId = newStatusId;
            await _repo.UpdateAsync(existing);
            return true;
        }
         public async Task<bool> DeleteAsync(PaymentDestination entity, short newStatusId)
        {
            var pd = await _repository.GetByIdAsync(entity.DestinationId);
            if (pd == null)
            {
                return false;
            }
            if (pd.Status.Name == StatusVariables.Draft)
            {
                await _repository.DeleteAsync(pd);
            }
            else
            {
                await UpdateStatusAsync(pd.DestinationId, newStatusId);
            }
            return true;
        }
    }
}
