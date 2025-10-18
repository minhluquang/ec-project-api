using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.payments;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.payments;
using ec_project_api.Interfaces.Payments;
using ec_project_api.Models;
using ec_project_api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ec_project_api.Facades.Payments
{
    public class PaymentDestinationFacade
    {
        private readonly IPaymentDestinationService _service;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public PaymentDestinationFacade(IPaymentDestinationService service, IStatusService statusService, IMapper mapper)
        {
            _service = service;
            _statusService = statusService;
            _mapper = mapper;
        }

        public async Task<PagedResult<PaymentDestinationDto>> GetAllPagedAsync(PaymentDestinationFilter filter)
        {
            var options = new ec_project_api.Repository.Base.QueryOptions<PaymentDestination>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            options.Filter = pd =>
                (!filter.StatusId.HasValue || pd.StatusId == filter.StatusId.Value) &&
                (string.IsNullOrEmpty(filter.Identifier) || pd.Identifier.Contains(filter.Identifier));

            if (!string.IsNullOrEmpty(filter.OrderBy))
            {
                switch (filter.OrderBy)
                {
                    case "id_desc":
                        options.OrderBy = q => q.OrderByDescending(p => p.DestinationId);
                        break;
                    case "id_asc":
                        options.OrderBy = q => q.OrderBy(p => p.DestinationId);
                        break;
                }
            }

            var paged = await _service.GetAllPagedAsync(options);

            var dtoItems = _mapper.Map<IEnumerable<PaymentDestinationDto>>(paged.Items);
            return new PagedResult<PaymentDestinationDto>
            {
                Items = dtoItems,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }

        public async Task<PaymentDestinationDto?> GetByIdAsync(int id)
        {
            var pd = await _service.GetByIdAsync(id);
            if (pd == null)
                throw new InvalidOperationException(PaymentMessages.NotFound);

            return _mapper.Map<PaymentDestinationDto>(pd);
        }

        public async Task<bool> CreateAsync(PaymentDestinationCreateRequest request)
        {
            var pd = _mapper.Map<PaymentDestination>(request);
            pd.CreatedAt = DateTime.UtcNow;
            pd.UpdatedAt = DateTime.UtcNow;

            var result = await _service.CreateAsync(pd);
            if (!result)
                throw new InvalidOperationException(PaymentMessages.PaymentDestinationCreatedFailed);

            return result;
        }

        public async Task<bool> UpdateAsync(int id, PaymentDestinationUpdateRequest request)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException(PaymentMessages.NotFound);

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            var result = await _service.UpdateAsync(existing);
            if (!result)
                throw new InvalidOperationException(PaymentMessages.PaymentDestinationUpdatedFailed);

            return result;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException(PaymentMessages.NotFound);

            var inactiveStatus = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.PaymentDestination &&
                     s.Name == StatusVariables.Inactive
            );

            if (inactiveStatus == null)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var result = await _service.DeleteAsync(existing, inactiveStatus.StatusId);
            if (!result)
                throw new InvalidOperationException(PaymentMessages.PaymentDestinationDeletedFailed);

            return result;
        }

        public async Task<bool> UpdateStatusAsync(int id, short newStatusId)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException(PaymentMessages.NotFound);

            var status = await _statusService.GetByIdAsync(newStatusId);
            if (status == null)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var result = await _service.UpdateStatusAsync(id, newStatusId);
            if (!result)
                throw new InvalidOperationException(PaymentMessages.PaymentDestinationStatusUpdatedFailed);

            return result;
        }
    }
}
