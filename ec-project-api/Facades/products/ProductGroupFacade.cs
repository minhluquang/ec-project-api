using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Dtos.request.productGroups;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Services;
using ec_project_api.Services.product_groups;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ec_project_api.Facades.productGroups
{
    public class ProductGroupFacade
    {
        private readonly IProductGroupService _productGroupService;
        private readonly IMapper _mapper;

        public ProductGroupFacade(IProductGroupService productGroupService, IMapper mapper)
        {
            _productGroupService = productGroupService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductGroupDto>> GetAllAsync()
        {
            var productGroups = await _productGroupService.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductGroupDto>>(productGroups);
        }

        public async Task<bool> CreateAsync(ProductGroupCreateRequest request)
        {
            var existing = await _productGroupService.FirstOrDefaultAsync(pg => pg.Name == request.Name.Trim());
            if (existing != null)
                throw new InvalidOperationException(ProductGroupMessages.ProductGroupNameAlreadyExists);

            var productGroup = _mapper.Map<ProductGroup>(request);
            productGroup.CreatedAt = DateTime.UtcNow;
            productGroup.UpdatedAt = DateTime.UtcNow;

            return await _productGroupService.CreateAsync(productGroup);
        }

        public async Task<bool> UpdateAsync(int id, ProductGroupUpdateRequest request)
        {
            var existing = await _productGroupService.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException(ProductGroupMessages.ProductGroupNotFound);

            var duplicate = await _productGroupService.FirstOrDefaultAsync(pg => pg.ProductGroupId != id && pg.Name == request.Name.Trim());
            if (duplicate != null)
                throw new InvalidOperationException(ProductGroupMessages.ProductGroupNameAlreadyExists);

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            return await _productGroupService.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var productGroup = await _productGroupService.GetByIdAsync(id);
            if (productGroup == null)
                throw new KeyNotFoundException(ProductGroupMessages.ProductGroupNotFound);

            return await _productGroupService.DeleteAsync(productGroup);
        }
    }
}