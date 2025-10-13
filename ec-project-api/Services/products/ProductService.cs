using ec_project_api.Constants.Messages;
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.response.products;
using ec_project_api.Dtos.Statuses;
using ec_project_api.Interfaces.Products;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using ec_project_api.Services.Bases;
using ec_project_api.Services.product_groups;
using ec_project_api.Services.product_images;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ec_project_api.Services {

    public class ProductService : BaseService<Product, int>, IProductService {
        private readonly IProductRepository _productRepository;
        private readonly IProductImageService _productImageService;
        private readonly ICategoryService _categoryService;
        private readonly IColorService _colorService;
        private readonly IProductGroupService _productGroupService;
        private readonly IStatusService _statusService;
        private readonly IMaterialService _materialService;

        public ProductService(
            IProductRepository productRepository,
            IProductImageService productImageService,
            IProductGroupService productGroupService,
            ICategoryService categoryService,
            IColorService colorService,
            IStatusService statusService,
            IMaterialService materialService
        ) : base(productRepository) {
            _productRepository = productRepository;
            _productImageService = productImageService;
            _colorService = colorService;
            _categoryService = categoryService;
            _productGroupService = productGroupService;
            _statusService = statusService;
            _materialService = materialService;
        }

        public override async Task<IEnumerable<Product>> GetAllAsync(QueryOptions<Product>? options = null) {
            options ??= new QueryOptions<Product>();

            options.Includes.Add(p => p.Category);
            options.Includes.Add(p => p.Material);
            options.Includes.Add(p => p.Status);
            options.Includes.Add(p => p.Color);
            options.Includes.Add(p => p.ProductImages.Where(pi => pi.IsPrimary));

            var products = await _productRepository.GetAllAsync(options);
            return products;
        }

        public override async Task<Product?> GetByIdAsync(int id, QueryOptions<Product>? options = null) {
            options ??= new QueryOptions<Product>();

            options.Includes.Add(p => p.Category);
            options.Includes.Add(p => p.Material);
            options.Includes.Add(p => p.Status);
            options.Includes.Add(p => p.Color);
            options.IncludeThen.Add(q => q
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Size));
            options.IncludeThen.Add(q => q
                .Include(p => p.ProductVariants)
                    .ThenInclude(pv => pv.Status));
            options.Includes.Add(p => p.ProductImages);

            var product = await _productRepository.GetByIdAsync(id, options);
            return product;
        }

        public async Task<bool> CreateAsync(Product product, ProductImage productImage, IFormFile FileImage) {
            await base.CreateAsync(product);

            productImage.ProductId = product.ProductId;

            var uploadResult = await _productImageService.UploadSingleProductImageAsync(productImage, FileImage);
            if (!uploadResult)
                throw new Exception("Upload ảnh thất bại");

            return true;
        }
        public async Task<IEnumerable<Product>> GetAllByCategoryidAsync(short categoryId, int? pageNumber = 1, int? pageSize = 12, decimal? minPrice = null, decimal? maxPrice = null, short? colorId = null, string? orderBy = null) {
            var options = new QueryOptions<Product>();

            options.Filter = p => p.CategoryId == categoryId &&
               (!minPrice.HasValue ||
                   (p.DiscountPercentage.HasValue
                       ? p.BasePrice - (p.BasePrice * p.DiscountPercentage.Value / 100)
                       : p.BasePrice) >= minPrice.Value) &&
               (!maxPrice.HasValue ||
                   (p.DiscountPercentage.HasValue
                       ? p.BasePrice - (p.BasePrice * p.DiscountPercentage.Value / 100)
                       : p.BasePrice) <= maxPrice.Value) &&
               (!colorId.HasValue || p.ColorId == colorId);

            if (!orderBy.IsNullOrEmpty()) {
                switch (orderBy) {
                    case "name_asc":
                        options.OrderBy = q => q.OrderBy(p => p.Name);
                        break;
                    case "name_desc":
                        options.OrderBy = q => q.OrderByDescending(p => p.Name);
                        break;
                    case "price_asc":
                        options.OrderBy = q => q.OrderBy(p => p.DiscountPercentage.HasValue ? p.BasePrice - (p.BasePrice * p.DiscountPercentage / 100) : p.BasePrice);
                        break;
                    case "price_desc":
                        options.OrderBy = q => q.OrderByDescending(p => p.DiscountPercentage.HasValue ? p.BasePrice - (p.BasePrice * p.DiscountPercentage / 100) : p.BasePrice);
                        break;
                    case "date_old_new":
                        options.OrderBy = q => q.OrderBy(p => p.CreatedAt);
                        break;
                    case "date_new_old":
                        options.OrderBy = q => q.OrderByDescending(p => p.CreatedAt);
                        break;
                }
            }

            options.PageNumber = pageNumber;
            options.PageSize = pageSize;

            options.Includes.Add(p => p.Category);
            options.Includes.Add(p => p.Material);
            options.Includes.Add(p => p.Status);
            options.Includes.Add(p => p.Color);
            options.Includes.Add(p => p.ProductImages.Where(pi => pi.IsPrimary));

            return await _productRepository.GetAllAsync(options);
        }

        public override async Task<bool> DeleteAsync(Product product) {
            var productImages = await _productImageService.GetAllByProductIdAsync(product.ProductId);
            foreach (var image in productImages) {
                await _productImageService.DeleteSingleProductImageAsync(image);
            }
            return await base.DeleteAsync(product);
        }

        public async Task<ProductFormMetaDto> GetProductFormMetaAsync() {

            var activeStatusCategory = await _statusService.FirstOrDefaultAsync(s => s.Name == StatusVariables.Active && s.EntityType == EntityVariables.Category) ?? throw new KeyNotFoundException(StatusMessages.StatusNotFound);
            var activeStatusColor = await _statusService.FirstOrDefaultAsync(s => s.Name == StatusVariables.Active && s.EntityType == EntityVariables.Color) ?? throw new KeyNotFoundException(StatusMessages.StatusNotFound);
            var activeStatusProductGroup = await _statusService.FirstOrDefaultAsync(s => s.Name == StatusVariables.Active && s.EntityType == EntityVariables.ProductGroup) ?? throw new KeyNotFoundException(StatusMessages.StatusNotFound);
            var activeStatusMaterial = await _statusService.FirstOrDefaultAsync(s => s.Name == StatusVariables.Active && s.EntityType == EntityVariables.Material) ?? throw new KeyNotFoundException(StatusMessages.StatusNotFound);

            var categories = await _categoryService.GetAllAsync(new QueryOptions<Category>
            {
                Filter = c => c.StatusId == activeStatusCategory.StatusId && c.ParentId != null
            });
            var colors = await _colorService.GetAllAsync(new QueryOptions<Color>
            {
                Filter = c => c.StatusId == activeStatusColor.StatusId
            });
            var productGroups = await _productGroupService.GetAllAsync(new QueryOptions<ProductGroup>
            {
                Filter = pg => pg.StatusId == activeStatusProductGroup.StatusId
            });
            var materials = await _materialService.GetAllAsync(new QueryOptions<Material>
            {
                Filter = m => m.StatusId == activeStatusMaterial.StatusId
            });
            var statuses = await _statusService.GetAllAsync(new QueryOptions<Status>
            {
                Filter = s => s.EntityType == EntityVariables.Product
            });

            return new ProductFormMetaDto
            {
                Categories = categories.Select(c => new CategoryDto { CategoryId = c.CategoryId, Name = c.Name }),
                Colors = colors.Select(c => new ColorDto { ColorId = c.ColorId, Name = c.Name }),
                Materials = materials.Select(m => new MaterialDto { MaterialId = m.MaterialId, Name = m.Name }),
                ProductGroups = productGroups.Select(pg => new ProductGroupDto { ProductGroupId = pg.ProductGroupId, Name = pg.Name }),
                Statuses = statuses.Select(s => new StatusDto { Name = s.Name, DisplayName = s.DisplayName, StatusId = s.StatusId, EntityType = s.EntityType})
            };
        }
    }
}