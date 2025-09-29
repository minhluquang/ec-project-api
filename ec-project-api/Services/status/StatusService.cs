using System.Linq.Expressions;
using ec_project_api.Constants.Messages;
using ec_project_api.Helpers;
using ec_project_api.Interfaces.System;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services
{
    public class StatusService : BaseService<Status, int>, IStatusService
    {
        public StatusService(IStatusRepository repository)
            : base(repository)
        {
        }

        public override async Task<IEnumerable<Status>> GetAllAsync(QueryOptions<Status>? options = null)
        {
            IEnumerable<Status> statuses;

            if (options?.Filter != null)
            {
                statuses = await _repository.FindAsync(options.Filter);
            }
            else
            {
                statuses = await _repository.FindAsync(s => !string.IsNullOrEmpty(s.EntityType));
            }

            if (statuses == null || !statuses.Any())
                throw new KeyNotFoundException(StatusMessages.StatusNotFound);

            return statuses;
        }

        public override async Task<Status?> GetByIdAsync(int id, QueryOptions<Status>? options = null)
        {
            Expression<Func<Status, bool>> idFilter = s => s.StatusId == id;

            var filter = options?.Filter != null
                ? options.Filter.AndAlso(idFilter)
                : idFilter;

            var status = await _repository.FirstOrDefaultAsync(filter, options);

            if (status == null)
                throw new KeyNotFoundException(StatusMessages.StatusNotFound);

            return status;
        }
    }
}
