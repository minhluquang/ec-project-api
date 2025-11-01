using ec_project_api.Constants.messages;
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

namespace ec_project_api.Services.products {
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

        public async Task<(Product product, IEnumerable<Product> related)> GetBySlugAsync(string slug)
        {
            var options = new QueryOptions<Product>
            {
                Filter = p => p.Slug == slug,
            };

            options.Includes.Add(p => p.Category);
            options.Includes.Add(p => p.Material);
            options.Includes.Add(p => p.Status);
            options.Includes.Add(p => p.Color);
            options.Includes.Add(p => p.ProductGroup); 
            options.IncludeThen.Add(q => q
                .Include(p => p.ProductVariants)
                .ThenInclude(pv => pv.Size));
            options.IncludeThen.Add(q => q
                .Include(p => p.ProductVariants)
                .ThenInclude(pv => pv.Status));
            options.Includes.Add(p => p.ProductImages);

            var products = await _productRepository.GetAllAsync(options);
            var product = products.FirstOrDefault();
            if (product == null)
                throw new KeyNotFoundException(ProductMessages.ProductNotFound);

            var relatedOptions = new QueryOptions<Product>
            {
                Filter = p => p.ProductGroupId == product.ProductGroupId && p.ProductId != product.ProductId
            };
            
            relatedOptions.Includes.Add(p => p.ProductImages);
            relatedOptions.Includes.Add(p => p.ProductGroup);
            var relatedProducts = await _productRepository.GetAllAsync(relatedOptions);
            
            return (product, relatedProducts);
        }
        
        public async Task<IEnumerable<Product>> SearchTop5Async(string search)
        {
            var options = new QueryOptions<Product>
            {
                Filter = p => (p.Name != null && p.Name.Contains(search)),
                OrderBy = q => q.OrderByDescending(p => p.CreatedAt)
            };

            options.Includes.Add(p => p.Category);
            options.Includes.Add(p => p.Material);
            options.Includes.Add(p => p.Status);
            options.Includes.Add(p => p.Color);
            options.Includes.Add(p => p.ProductImages.Where(pi => pi.IsPrimary));

            var products = await _productRepository.GetAllAsync(options);
            return products.Take(5);
        }

        public async Task<bool> CreateAsync(Product product, ProductImage productImage, IFormFile FileImage) {
            await base.CreateAsync(product);

            productImage.ProductId = product.ProductId;

            var uploadResult = await _productImageService.UploadSingleProductImageAsync(productImage, FileImage);
            if (!uploadResult)
                throw new Exception("Upload ảnh thất bại");

            return true;
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

        public async Task<ProductFilterOptionDto> GetFilterOptionsByCategorySlugAsync(string? categorySlug, string? search)
        {
            int? categoryId = null;
    
            // Xử lý category (optional)
            if (!string.IsNullOrWhiteSpace(categorySlug))
            {
                var category = await _categoryService.FirstOrDefaultAsync(c => c.Slug == categorySlug);
                if (category == null)
                    throw new InvalidOperationException(CategoryMessages.CategoryNotFound);
                categoryId = category.CategoryId;
            }
            
            var activeStatus = await _statusService.FirstOrDefaultAsync(
                s => s.EntityType == EntityVariables.Product && s.Name == StatusVariables.Active)
                ?? throw new InvalidOperationException(StatusMessages.StatusNotFound);

            var options = new QueryOptions<Product>
            {
                Filter = p =>
                    // Category filter (optional)
                    (!categoryId.HasValue || p.CategoryId == categoryId.Value) &&
            
                    // Status filter
                    p.Status != null && p.Status.StatusId == activeStatus.StatusId &&
            
                    // Search filter (optional)
                    (string.IsNullOrEmpty(search) ||
                     (p.Name != null && p.Name.Contains(search)) ||
                     (p.Slug != null && p.Slug.Contains(search)))
            };

            var products = await _productRepository.GetAllAsync(options);

            var colorIds = products.Select(p => p.ColorId).Distinct().ToList();
            var materialIds = products.Select(p => p.MaterialId).Distinct().ToList();
            var productGroupIds = products.Select(p => p.ProductGroupId).Distinct().ToList();

            var colors = await _colorService.GetAllAsync(new QueryOptions<Color>
            {
                Filter = c => colorIds.Contains(c.ColorId)
            });
            var materials = await _materialService.GetAllAsync(new QueryOptions<Material>
            {
                Filter = m => materialIds.Contains(m.MaterialId)
            });
            var productGroups = await _productGroupService.GetAllAsync(new QueryOptions<ProductGroup>
            {
                Filter = pg => productGroupIds.Contains(pg.ProductGroupId)
            });

            // Count stock status in ProductVariants.StockQuantity
            var inStockCount = products.Count(p => p.ProductVariants != null && p.ProductVariants.Any(v => v.StockQuantity > 0));
            var outOfStockCount = products.Count(p => p.ProductVariants != null && p.ProductVariants.All(v => v.StockQuantity == 0));

            var stockStatuses = new List<StockStatusDto>
            {
                new StockStatusDto
                {
                    Label = "In stock",
                    InStock = true,
                    ProductCount = inStockCount
                },
                new StockStatusDto
                {
                    Label = "Out of stock",
                    InStock = false,
                    ProductCount = outOfStockCount
                }
            };

            var result = new ProductFilterOptionDto
            {
                ColorOptions = colors.Select(c => new ColorStatDto
                {
                    ColorId = c.ColorId,
                    Name = c.Name,
                    ProductCount = products.Count(p => p.ColorId == c.ColorId)
                }),
                MaterialOptions = materials.Select(m => new MaterialStatDto
                {
                    MaterialId = m.MaterialId,
                    Name = m.Name,
                    ProductCount = products.Count(p => p.MaterialId == m.MaterialId)
                }),
                ProductGroupOptions = productGroups.Select(pg => new ProductGroupStatDto
                {
                    ProductGroupId = pg.ProductGroupId,
                    Name = pg.Name,
                    ProductCount = products.Count(p => p.ProductGroupId == pg.ProductGroupId)
                }),
                StockStatusOptions = stockStatuses
            };

            return result;
        }
    }
}