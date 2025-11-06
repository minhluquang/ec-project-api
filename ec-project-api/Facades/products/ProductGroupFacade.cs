using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.productGroups;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services;
using ec_project_api.Services.product_groups;
using System.Linq.Expressions;

namespace ec_project_api.Facades.products
{
    public class ProductGroupFacade
    {
        private readonly IProductGroupService _productGroupService;
        private readonly IStatusService _statusService;
        private readonly IMapper _mapper;

        public ProductGroupFacade(IProductGroupService productGroupService, IStatusService statusService, IMapper mapper)
        {
            _productGroupService = productGroupService;
            _statusService = statusService; // keep consistent field name as in other facades if different, adjust if needed
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductGroupDto>> GetAllAsync()
        {
            var productGroups = await _productGroupService.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductGroupDto>>(productGroups);
        }

        public async Task<ProductGroupDetailDto> GetByIdAsync(int id)
        {
            var productGroup = await _productGroupService.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(ProductGroupMessages.ProductGroupNotFound);

            return _mapper.Map<ProductGroupDetailDto>(productGroup);
        }

        public async Task<bool> CreateAsync(ProductGroupCreateRequest request)
        {
            var existing = await _productGroupService.FirstOrDefaultAsync(pg => pg.Name == request.Name.Trim());
            if (existing != null)
                throw new InvalidOperationException(ProductGroupMessages.ProductGroupNameAlreadyExists);

            var inActiveStatus = await _statusService.FirstOrDefaultAsync(s => s.Name == "Inactive" && s.EntityType == "ProductGroup")
              ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var productGroup = _mapper.Map<ProductGroup>(request);
            productGroup.StatusId = inActiveStatus.StatusId;
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

            var status = await _statusService.GetByIdAsync(request.StatusId);
            if (status == null || status.EntityType != EntityVariables.ProductGroup)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            if (status.Name == StatusVariables.Inactive)
            {
                // Kiểm tra có sản phẩm nào đang active mà thuộc về product group này không
                if (existing.Products != null && existing.Products.Any(p => p.Status.EntityType == EntityVariables.Product && p.Status.Name == StatusVariables.Active))
                {
                    throw new InvalidOperationException(ProductGroupMessages.ProductGroupUpdateStatusFailedProductActive);
                }
            }

            _mapper.Map(request, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            return await _productGroupService.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var productGroup = await _productGroupService.GetByIdAsync(id);
            if (productGroup == null)
                throw new KeyNotFoundException(ProductGroupMessages.ProductGroupNotFound);

            if (productGroup.Status.Name != StatusVariables.Inactive)
                throw new InvalidOperationException(ProductGroupMessages.ProductGroupDeleteFailedNotInactive);

            // Kiểm tra có sản phẩm nào dùng nhóm sản phẩm này không
            if (productGroup.Products != null && productGroup.Products.Any())
                throw new InvalidOperationException(ProductGroupMessages.ProductGroupInUse);

            return await _productGroupService.DeleteAsync(productGroup);
        }

        private static Expression<Func<ProductGroup, bool>> BuildProductGroupFilter(ProductGroupFilter filter)
        {
            return pg =>
                (string.IsNullOrEmpty(filter.StatusName) ||
                    (pg.Status != null && pg.Status.Name == filter.StatusName && pg.Status.EntityType == EntityVariables.ProductGroup)) &&
                (string.IsNullOrEmpty(filter.Search) ||
                    pg.Name.Contains(filter.Search) ||
                    pg.ProductGroupId.ToString().Contains(filter.Search) ||
                    pg.ProductGroupId.ToString().Contains(filter.Search));
        }

        public async Task<PagedResult<ProductGroupDetailDto>> GetAllPagedAsync(ProductGroupFilter filter)
        {
            var options = new QueryOptions<ProductGroup>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Filter = BuildProductGroupFilter(filter)
            };

            var pagedResult = await _productGroupService.GetAllPagedAsync(options);

            var dtoList = _mapper.Map<IEnumerable<ProductGroupDetailDto>>(pagedResult.Items);
            var pagedResultDto = new PagedResult<ProductGroupDetailDto>
            {
                Items = dtoList,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
            return pagedResultDto;
        }
    }
}