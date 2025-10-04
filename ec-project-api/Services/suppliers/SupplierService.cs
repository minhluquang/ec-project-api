using ec_project_api.Dtos.request.suppliers;
using ec_project_api.Dtos.response;
using ec_project_api.Interfaces.Suppliers;
using ec_project_api.Models;
using ec_project_api.Services.Bases;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Services.suppliers
{
    public class SupplierService : BaseService<Supplier, int>, ISupplierService
    {
        private readonly ISupplierRepository _repository;

        public SupplierService(ISupplierRepository repository)
            : base(repository)
        {
            _repository = repository;
        }

        public async Task<PagedResponse<Supplier>> GetPagedAsync(SupplierQueryRequest filter)
        {
            var query = _repository.Query();

            if (!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(s => s.Name.Contains(filter.Search));
            }

            // Lọc theo trạng thái
           if (filter.Status.HasValue)
            {
                query = query.Where(s => s.StatusId == filter.Status.Value);
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponse<Supplier>(
                items,
                totalItems,
                filter.Page,
                filter.PageSize
            );
        }
    }
}
