using ec_project_api.Interfaces.Suppliers;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;

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

        public override async Task<IEnumerable<Supplier>> GetAllAsync(QueryOptions<Supplier>? options = null)
        {
            options ??= new QueryOptions<Supplier>();
            options.Includes.Add(s => s.Status);
            short? statusId = null;
            options.Filter = s => !statusId.HasValue || s.StatusId == statusId.Value;
            return await base.GetAllAsync(options);
        }
        public override async Task<Supplier?> GetByIdAsync(int id, QueryOptions<Supplier>? options = null)
        {
            options ??= new QueryOptions<Supplier>();
            options.Includes.Add(s => s.Status);
            return await base.GetByIdAsync(id, options);
        }
        public async Task<bool> UpdateStatusAsync(int id, short newStatusId)
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