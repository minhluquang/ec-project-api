using AutoMapper;
using ec_project_api.Constants.messages;
using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.request.products;
using ec_project_api.Dtos.response.pagination;
using ec_project_api.Dtos.response.products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services;
using ec_project_api.Services.product_groups;
using System.Linq.Expressions;
using ec_project_api.Services.order_items;
using ec_project_api.Services.reviews;
using Microsoft.EntityFrameworkCore;

namespace ec_project_api.Facades.products {
    public class ProductFacade {
        private readonly IProductService _productService;
        private readonly IStatusService _statusService;
        private readonly IMaterialService _materialService;
        private readonly ICategoryService _categoryService;
        private readonly IProductGroupService _productGroupService;
        private readonly IColorService _colorService;
        private readonly IReviewService _reviewService;
        private readonly IOrderItemService _orderItemService;
        private readonly IMapper _mapper;

        public ProductFacade(IProductService productService, IOrderItemService orderItemService,  IReviewService reviewService, IStatusService statusService, IMaterialService materialService, ICategoryService categoryService, IProductGroupService productGroupService, IColorService colorService, IMapper mapper) {
            _productService = productService;
            _statusService = statusService;
            _categoryService = categoryService;
            _materialService = materialService;
            _productGroupService = productGroupService;
            _colorService = colorService;
            _reviewService = reviewService;
            _orderItemService = orderItemService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync() {
            var products = await _productService.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);  
        }

        public async Task<ProductDetailDto> GetBySlugAsync(string slug) {
            var (product, related) = await _productService.GetBySlugAsync(slug);
            
            // If no variants or status is Draft (or status missing), treat as not found
            if (product == null ||
                product.ProductVariants == null || !product.ProductVariants.Any() ||
                product.Status == null || product.Status.Name == StatusVariables.Draft || product.Status.Name == StatusVariables.Inactive) {
                throw new KeyNotFoundException(ProductMessages.ProductNotFound);
            }
            
            var reviewSummary = await _reviewService.GetSummaryByProductIdAsync(product.ProductId);
            var soldQuantity = await _orderItemService.GetSoldQuantityByProductIdAsync(product.ProductId);

            var dto = _mapper.Map<ProductDetailDto>(product);
            dto.RelatedProducts = _mapper.Map<IEnumerable<ProductDto>>(related);
            dto.Rating = reviewSummary.AverageRating;
            dto.ReviewCount = reviewSummary.ReviewCount;
            dto.ReviewDetails = reviewSummary.ReviewDetails;
            dto.HasImageCount = reviewSummary.HasImageCount;
            dto.SoldQuantity = soldQuantity;
            
            foreach (var relatedDto in dto.RelatedProducts) {
                var relatedProduct = related.FirstOrDefault(p => p.ProductId == relatedDto.ProductId);
                if (relatedProduct?.ProductVariants != null) {
                    relatedDto.OutOfStock = relatedProduct.ProductVariants
                        .Where(pv => pv.Status?.Name == StatusVariables.Active)
                        .All(pv => pv.StockQuantity == 0);
                }
            }
            
            return dto;
        }
        
        public async Task<IEnumerable<ProductDto>> SearchTop5Async(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return Enumerable.Empty<ProductDto>();
        
            var products = await _productService.SearchTop5Async(search);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
        

        public async Task<bool> CreateAsync(ProductCreateRequest request) {
            var existingProduct = await _productService.FirstOrDefaultAsync(p => (p.Name == request.Name.Trim() && p.CategoryId == request.CategoryId && p.MaterialId == request.MaterialId && p.ColorId == request.ColorId) || p.Slug == request.Slug);

            if (existingProduct != null) {
                if (existingProduct.Slug == request.Slug)
                    throw new InvalidOperationException(ProductMessages.ProductSlugAlreadyExists);
                else
                    throw new InvalidOperationException(ProductMessages.ProductAlreadyExistsWithNameCategoryMaterial);
            }

            var productGroup = await _productGroupService.GetByIdAsync(request.ProductGroupId);
            if (productGroup == null)
                throw new InvalidOperationException(ProductMessages.ProductGroupNotFound);

            var material = await _materialService.GetByIdAsync(request.MaterialId);
            if (material == null)
                throw new InvalidOperationException(MaterialMessages.MaterialNotFound);

            var category = await _categoryService.GetByIdAsync(request.CategoryId);
            if (category == null)
                throw new InvalidOperationException(CategoryMessages.CategoryNotFound);

            var color = await _colorService.GetByIdAsync(request.ColorId);
                if (color == null)
                throw new InvalidOperationException(ColorMessages.ColorNotFound);

            var draftStatus = await _statusService.FirstOrDefaultAsync(s =>
                    s.EntityType == EntityVariables.Product && s.Name == StatusVariables.Draft);
            if (draftStatus == null)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var product = _mapper.Map<Product>(request);
            product.StatusId = draftStatus.StatusId;
            product.DiscountPercentage = 0;
            product.BasePrice = 0;

            var productImage = new ProductImage
            {
                AltText = request.AltText,
                IsPrimary = true,
                DisplayOrder = 1
            };

            return await _productService.CreateAsync(product, productImage, request.FileImage);
        }

        public async Task<bool> UpdateAsync(int id, ProductUpdateRequest request) {
            var existingProduct = await _productService.FirstOrDefaultAsync(
                p => (p.ProductId != id) &&
                     (
                        (p.Name == request.Name.Trim() && p.CategoryId == request.CategoryId && p.MaterialId == request.MaterialId)
                        || (p.Slug == request.Slug)
                     )
            );

            if (existingProduct != null)
                if (existingProduct.Slug == request.Slug)
                    throw new InvalidOperationException(ProductMessages.ProductSlugAlreadyExists);
                else
                    throw new InvalidOperationException(ProductMessages.ProductAlreadyExists);

            var currentProduct = await _productService.GetByIdAsync(id);
            if (currentProduct == null)
                throw new KeyNotFoundException(ProductMessages.ProductAlreadyExists);

            var material = await _materialService.GetByIdAsync(request.MaterialId);
            if (material == null)
                throw new InvalidOperationException(MaterialMessages.MaterialNotFound);

            var category = await _categoryService.GetByIdAsync(request.CategoryId);
            if (category == null)
                throw new InvalidOperationException(CategoryMessages.CategoryNotFound);

            var productGroup = await _productGroupService.GetByIdAsync(request.ProductGroupId);
            if (productGroup == null)
                throw new InvalidOperationException(ProductMessages.ProductGroupNotFound);
            
            var color = await _colorService.GetByIdAsync(request.ColorId);
            if (color == null)
                throw new InvalidOperationException(ColorMessages.ColorNotFound);

            var requestedStatus = await _statusService.GetByIdAsync(request.StatusId);
            if (requestedStatus == null || requestedStatus.EntityType != EntityVariables.Product)
                throw new InvalidOperationException(StatusMessages.StatusNotFound);
            
            // If product is currently Inactive (or missing status) and request wants Active,
            // ensure related entities are Active (promote them if they are Inactive).
            if ((currentProduct.Status == null || currentProduct.Status.Name == StatusVariables.Inactive) &&
                requestedStatus.Name == StatusVariables.Active)
            {
                var activeMaterialStatus = await _statusService.GetByNameAndEntityTypeAsync(StatusVariables.Active, EntityVariables.Material)
                    ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);
                var activeCategoryStatus = await _statusService.GetByNameAndEntityTypeAsync(StatusVariables.Active, EntityVariables.Category)
                    ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);
                var activeProductGroupStatus = await _statusService.GetByNameAndEntityTypeAsync(StatusVariables.Active, EntityVariables.ProductGroup)
                    ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);
                var activeColorStatus = await _statusService.GetByNameAndEntityTypeAsync(StatusVariables.Active, EntityVariables.Color)
                    ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);
            
                if (material.StatusId != activeMaterialStatus.StatusId)
                {
                    material.StatusId = activeMaterialStatus.StatusId;
                    if (!await _materialService.UpdateAsync(material))
                        throw new InvalidOperationException(MaterialMessages.MaterialUpdateFailed);
                }
            
                if (category.StatusId != activeCategoryStatus.StatusId)
                {
                    category.StatusId = activeCategoryStatus.StatusId;
                    if (!await _categoryService.UpdateAsync(category))
                        throw new InvalidOperationException(CategoryMessages.CategoryUpdateFailed);
                }
            
                if (productGroup.StatusId != activeProductGroupStatus.StatusId)
                {
                    productGroup.StatusId = activeProductGroupStatus.StatusId;
                    if (!await _productGroupService.UpdateAsync(productGroup))
                        throw new InvalidOperationException(ProductMessages.ProductGroupUpdateFailed);
                }
            
                if (color.StatusId != activeColorStatus.StatusId)
                {
                    color.StatusId = activeColorStatus.StatusId;
                    if (!await _colorService.UpdateAsync(color))
                        throw new InvalidOperationException(ColorMessages.ColorUpdateFailed);
                }
            }
            
            _mapper.Map(request, currentProduct);
            currentProduct.UpdatedAt = DateTime.UtcNow;
            
            return await _productService.UpdateAsync(currentProduct);
        }

        // public async Task<IEnumerable<ProductDto>> GetAllByCategorySlugAsync(string categorySlug) {
        //     if (pageNumber <= 0 && pageNumber.HasValue)
        //         throw new ArgumentException("Số trang phải lớn hơn 0", nameof(pageNumber));
        //
        //     if ((pageSize <= 0 || pageSize > 100) && pageSize.HasValue)
        //         throw new ArgumentException("Kích thước trang phải từ 1 đến 100", nameof(pageSize));
        //
        //     if (minPrice.HasValue && minPrice.Value < 0)
        //         throw new ArgumentException("Giá tối thiểu phải lớn hơn hoặc bằng 0", nameof(minPrice));
        //
        //     if (maxPrice.HasValue && maxPrice.Value < 0)
        //         throw new ArgumentException("Giá tối đa phải lớn hơn hoặc bằng 0", nameof(maxPrice));
        //
        //     if (minPrice.HasValue && maxPrice.HasValue && minPrice.Value > maxPrice.Value)
        //         throw new ArgumentException("Giá tối thiểu không thể lớn hơn giá tối đa");
        //
        //     var category = await _categoryService.GetByIdAsync(categoryId);
        //     if (category == null) throw new InvalidOperationException(CategoryMessages.CategoryNotFound);
        //
        //     var products = await _productService.GetAllByCategoryidAsync(categoryId, pageNumber, pageSize, minPrice, maxPrice, colorId, orderBy);
        //     return _mapper.Map<IEnumerable<ProductDto>>(products);
        // }

        public async Task<bool> DeleteAsync(int productId) {
            var product = await _productService.GetByIdAsync(productId) ??
                throw new KeyNotFoundException(ProductMessages.ProductNotFound);

            if (product.Status.Name != StatusVariables.Draft)
                throw new InvalidOperationException(ProductMessages.ProductVariantDeleteFailedNotDraft);

            return await _productService.DeleteAsync(product);
        }

        public async Task<ProductFormMetaDto> GetProductFormMetaAsync() {
            return await _productService.GetProductFormMetaAsync();
        }

        private static Expression<Func<Product, bool>> BuildProductFilter(ProductFilter filter)
        {
            int? searchProductId = null;

            return p =>
                (string.IsNullOrEmpty(filter.StatusName) ||
                 (p.Status != null &&
                  p.Status.Name == filter.StatusName &&
                  p.Status.EntityType == EntityVariables.Product)) &&

                (string.IsNullOrEmpty(filter.Search) ||
                 (p.Name != null && p.Name.Contains(filter.Search)) ||
                 (p.Slug != null && p.Slug.Contains(filter.Search)) ||
                 p.ProductId.ToString().Contains(filter.Search) ||
                 (searchProductId.HasValue && p.ProductId == searchProductId.Value)) &&

                (!filter.ProductGroupId.HasValue || p.ProductGroupId == filter.ProductGroupId.Value) &&
                (!filter.CategoryId.HasValue || p.CategoryId == filter.CategoryId.Value) &&
                (!filter.ColorId.HasValue || p.ColorId == filter.ColorId.Value) &&
                (!filter.MaterialId.HasValue || p.MaterialId == filter.MaterialId.Value);
        }


        public async Task<PagedResult<ProductDto>> GetAllPagedAsync(ProductFilter filter) {
            var options = new QueryOptions<Product>
            {
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
            };

            options.Filter = BuildProductFilter(filter);

            var pagedResult = await _productService.GetAllPagedAsync(options);

            var dtoList = _mapper.Map<IEnumerable<ProductDto>>(pagedResult.Items);
            var pagedResultDto = new PagedResult<ProductDto>
            {
                Items = dtoList,
                TotalCount = pagedResult.TotalCount,
                TotalPages = pagedResult.TotalPages,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
            return pagedResultDto;
        }
        
        private static Expression<Func<Product, bool>> BuildProductFilterByCategorySlug(int? categoryId, ProductCategorySlugFilter filter)
        {
            var searchTerms = string.IsNullOrWhiteSpace(filter.Search)
                ? Array.Empty<string>()
                : filter.Search
                    .ToLower()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return p =>
                p.Status != null && p.Status.Name == StatusVariables.Active &&
                p.ProductVariants != null && p.ProductVariants.Any(pv => pv.Status.Name == StatusVariables.Active) &&

                // Category filter (optional)
                (!categoryId.HasValue || (p.Category != null && p.Category.CategoryId == categoryId.Value)) &&

                // Search filter (grouped) - close this group so following filters are applied with AND
                (searchTerms.Length == 0 ||
                 searchTerms.All(term =>
                     EF.Functions.Like(p.Name, "%" + term + "%") ||
                     EF.Functions.Like(p.Slug, "%" + term + "%")
                 )
                ) &&
                
                // Color filter
                (filter.ColorIds == null || filter.ColorIds.Count == 0 || filter.ColorIds.Contains(p.ColorId)) &&

                // Material filter
                (filter.MaterialIds == null || filter.MaterialIds.Count == 0 ||
                 filter.MaterialIds.Contains(p.MaterialId)) &&

                // Product Group filter
                (filter.ProductGroupIds == null || filter.ProductGroupIds.Count == 0 ||
                 filter.ProductGroupIds.Contains(p.ProductGroupId)) &&

                // Price filters
                (!filter.MinPrice.HasValue ||
                 ((p.DiscountPercentage.HasValue
                     ? p.BasePrice - (p.BasePrice * p.DiscountPercentage.Value / 100)
                     : p.BasePrice) >= filter.MinPrice.Value)) &&

                (!filter.MaxPrice.HasValue ||
                 ((p.DiscountPercentage.HasValue
                     ? p.BasePrice - (p.BasePrice * p.DiscountPercentage.Value / 100)
                     : p.BasePrice) <= filter.MaxPrice.Value)) &&

                // Stock filters - Chỉ apply khi ĐÚNG 1 trong 2 = true
                (
                    // Không có filter nào active -> show tất cả
                    (filter.OutOfStock != true && filter.InStock != true) ||

                    // Cả 2 đều true -> show tất cả
                    (filter.OutOfStock == true && filter.InStock == true) ||

                    // Chỉ OutOfStock = true
                    (filter.OutOfStock == true && filter.InStock != true &&
                     p.ProductVariants != null && p.ProductVariants.All(v => v.StockQuantity == 0)) ||

                    // Chỉ InStock = true
                    (filter.InStock == true && filter.OutOfStock != true &&
                     p.ProductVariants != null && p.ProductVariants.Any(v => v.StockQuantity > 0))
                );
        }
        
        private static Func<IQueryable<Product>, IOrderedQueryable<Product>> BuildProductOrderBy(string? orderBy)
        {
            if (string.IsNullOrEmpty(orderBy))
                return q => q.OrderByDescending(p => p.CreatedAt); // mặc định

            switch (orderBy.ToLower())
            {
                case "az":
                    return q => q.OrderBy(p => p.Name);

                case "za":
                    return q => q.OrderByDescending(p => p.Name);

                case "price_asc":
                    return q => q.OrderBy(p =>
                        p.DiscountPercentage.HasValue
                            ? p.BasePrice - (p.BasePrice * p.DiscountPercentage.Value / 100)
                            : p.BasePrice);

                case "price_desc":
                    return q => q.OrderByDescending(p =>
                        p.DiscountPercentage.HasValue
                            ? p.BasePrice - (p.BasePrice * p.DiscountPercentage.Value / 100)
                            : p.BasePrice);

                case "date_oldest":
                    return q => q.OrderBy(p => p.CreatedAt);

                case "date_newest":
                    return q => q.OrderByDescending(p => p.CreatedAt);
                
                // case "bestseller":
                //     return q => q.OrderByDescending(p => p.SoldCount);

                default:
                    return q => q.OrderByDescending(p => p.CreatedAt);
            }
        }
        
        public async Task<PagedResultWithCategory<ProductDto>> GetAllByCategorySlugPagedAsync(ProductCategorySlugFilter filter) {
            if (string.IsNullOrWhiteSpace(filter.CategorySlug) && string.IsNullOrWhiteSpace(filter.Search))
                    throw new ArgumentException(CategoryMessages.CategoryOrSearchRequired);
            
            int? categoryId = null;
            string? categoryName = null;

            if (!string.IsNullOrWhiteSpace(filter.CategorySlug))
            {
                var category = await _categoryService.FirstOrDefaultAsync(c => c.Slug == filter.CategorySlug);
                if (category == null || category.Status.Name == StatusVariables.Inactive)
                    throw new InvalidOperationException(CategoryMessages.CategoryNotFound);
                categoryId = category.CategoryId;
                categoryName = category.Name;
            }
            
             var options = new QueryOptions<Product>
             {
                 PageNumber = filter.PageNumber,
                 PageSize = filter.PageSize,
                 Filter = BuildProductFilterByCategorySlug(categoryId, filter),
                 OrderBy = BuildProductOrderBy(filter.OrderBy)
             };
             
             var pagedResult = await _productService.GetAllPagedAsync(options);
 
             var dtoList = pagedResult.Items
                 .Select(p =>
                 {
                     var dto = _mapper.Map<ProductDto>(p);
                     dto.OutOfStock = p.ProductVariants != null && p.ProductVariants.All(v => v.StockQuantity == 0);
                     return dto;
                 })
                 .ToList();
             
             return new PagedResultWithCategory<ProductDto>
             {
                 CategoryName = categoryName,
                 Items = dtoList,
                 TotalCount = pagedResult.TotalCount,
                 TotalPages = pagedResult.TotalPages,
                 PageNumber = pagedResult.PageNumber,
                 PageSize = pagedResult.PageSize
             };
        }

        public async Task<ProductFilterOptionDto> GetFilterOptionsByCategorySlugAsync(string? categorySlug, string? search)
        {
            if (string.IsNullOrWhiteSpace(categorySlug) && string.IsNullOrWhiteSpace(search))
                throw new ArgumentException(CategoryMessages.CategoryOrSearchRequired);
            
            var result = await _productService.GetFilterOptionsByCategorySlugAsync(categorySlug, search);
            return result;
        }
        
        public async Task<IEnumerable<ProductDto>> GetTopByCategoryExcludingProductAsync(short categoryId, int productId)
        {
            var category = await _categoryService.GetByIdAsync(categoryId);
            if (category == null)
                throw new InvalidOperationException(CategoryMessages.CategoryNotFound);

            var products = await _productService.GetTopByCategoryExcludingProductAsync(categoryId, productId, 10);
            var result = new List<ProductDto>();
            foreach (var p in products)
            {
                var dto = _mapper.Map<ProductDto>(p);

                if (p.ProductVariants == null)
                {
                    var loaded = await _productService.GetByIdAsync(p.ProductId);
                    dto.OutOfStock = loaded?.ProductVariants != null
                        ? loaded.ProductVariants.Where(v => v.Status?.Name == StatusVariables.Active).All(v => v.StockQuantity == 0)
                        : true; // fallback: treat missing data as out of stock
                }
                else
                {
                    dto.OutOfStock = p.ProductVariants
                        .Where(v => v.Status?.Name == StatusVariables.Active)
                        .All(v => v.StockQuantity == 0);
                }

                result.Add(dto);
            }

            return result;
        }
    }
}
