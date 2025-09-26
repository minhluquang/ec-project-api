using AutoMapper;
using ec_project_api.Dtos.Statuses;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services;

namespace ec_project_api.Facades
{
    public class StatusFacade
    {
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public StatusFacade(IStatusService statusService, IMapper mapper)
        {
            _statusService = statusService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StatusDto>> GetAllAsync(string? entityType)
        {
            var options = new QueryOptions<Status>();
            if (!string.IsNullOrEmpty(entityType))
            {
                options.Filter = s => s.EntityType == entityType;
            }

            var statuses = await _statusService.GetAllAsync(options);
            return _mapper.Map<IEnumerable<StatusDto>>(statuses);
        }

        public async Task<StatusDto> GetByIdAsync(int id, string? entityType)
        {
            var options = new QueryOptions<Status>();
            if (!string.IsNullOrEmpty(entityType))
            {
                options.Filter = s => s.EntityType == entityType;
            }

            var status = await _statusService.GetByIdAsync(id, options);
            return _mapper.Map<StatusDto>(status);
        }
    }
}
