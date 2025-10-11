using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Services.product_groups {
    public class ProductGroupService : BaseService<ProductGroup, int>, IProductGroupService {
        private readonly IProductGroupRepository _productGroupRepository;

        public ProductGroupService(IProductGroupRepository repository) : base(repository) {
            _productGroupRepository = repository;
        }
    }
}