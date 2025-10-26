using System.Linq.Expressions;
using ec_project_api.Constants.Messages;
using ec_project_api.Helpers;
using ec_project_api.Interfaces.System;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

namespace ec_project_api.Services
{
    public class StatusService : BaseService<Status, short>, IStatusService
    {
        public StatusService(IStatusRepository repository)
            : base(repository) { }

        public override async Task<IEnumerable<Status>> GetAllAsync(QueryOptions<Status>? options = null)
        {
            var filter = options?.Filter ?? (s => !string.IsNullOrEmpty(s.EntityType));
            var statuses = await _repository.FindAsync(filter);

            if (!statuses.Any())
                throw new KeyNotFoundException(StatusMessages.StatusNotFound);

            return statuses;
        }

        public override async Task<Status?> GetByIdAsync(short id, QueryOptions<Status>? options = null)
        {
            var idFilter = (Expression<Func<Status, bool>>)(s => s.StatusId == id);
            var filter = options?.Filter != null ? options.Filter.AndAlso(idFilter) : idFilter;

            return await _repository.FirstOrDefaultAsync(filter, options)
                ?? throw new KeyNotFoundException(StatusMessages.StatusNotFound);
        }
        public async Task<Status?> GetByNameAndEntityTypeAsync(string name, string entityType)
        {
            var options = new QueryOptions<Status>
            {
                Filter = s => s.Name == name && s.EntityType == entityType
            };
            var statuses = await _repository.GetAllAsync(options);
            return statuses.FirstOrDefault();
        }

    }
}
