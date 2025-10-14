using ec_project_api.Dtos.request.suppliers;
using ec_project_api.Dtos.response;
using ec_project_api.Interfaces.Suppliers;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Services.suppliers
{
    public class SupplierService : BaseService<Supplier, int>, ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
            : base(supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync(
            int? pageNumber = 1,
            int? pageSize = 10,
            int? statusId = null,
            string? name = null,
            string? orderBy = null)
        {
            var options = new QueryOptions<Supplier>();

            // lọc
            options.Filter = s =>
                (!statusId.HasValue || s.StatusId == statusId) &&
                (string.IsNullOrEmpty(name) || s.Name.Contains(name));

            // sắp xếp
            if (!string.IsNullOrEmpty(orderBy))
            {
                switch (orderBy)
                {
                    case "name_asc":
                        options.OrderBy = q => q.OrderBy(s => s.Name);
                        break;
                    case "name_desc":
                        options.OrderBy = q => q.OrderByDescending(s => s.Name);
                        break;
                }
            }
            // phân trang
            options.PageNumber = pageNumber;
            options.PageSize = pageSize;
            // include
            options.Includes.Add(s => s.Status);

            return await base.GetAllAsync(options);
        }

        public override async Task<Supplier?> GetByIdAsync(int id, QueryOptions<Supplier>? options = null)
        {
            options ??= new QueryOptions<Supplier>();
            options.Includes.Add(s => s.Status);
            return await base.GetByIdAsync(id, options);
        }
        public async Task<bool> UpdateStatusAsync(int id, int newStatusId)
        {
            var supplier = await _repository.GetByIdAsync(id);
            if (supplier == null)
                return false;

            supplier.StatusId = newStatusId;
            await _repository.UpdateAsync(supplier);
            return true;
        }

    }
}