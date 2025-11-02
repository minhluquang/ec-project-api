using System.Linq;
using Microsoft.EntityFrameworkCore;
using ec_project_api.Dtos.response.homepage;
using ec_project_api.Interfaces.Products;
using ec_project_api.Interfaces.Orders;
using ec_project_api.Interfaces;
using ec_project_api.Models;
using ec_project_api.Repository.Base;

namespace ec_project_api.Services.homepage
{
    public class HomepageService : IHomepageService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IOrderRepository _orderRepository;

        public HomepageService(IProductRepository productRepository, ICategoryRepository categoryRepository, IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _orderRepository = orderRepository;
        }

        public async Task<HomepageDto> GetHomepageDataAsync()
        {
            var categories = await GetCategoriesAsync();
            var bestSelling = await GetBestSellingProductsAsync();
            var onSale = await GetOnSaleProductsAsync();
            var bestSellingCategories = await GetBestSellingCategoriesAsync();

            return new HomepageDto
            {
                Categories = categories,
                BestSellingProducts = bestSelling,
                OnSaleProducts = onSale,
                BestSellingCategories = bestSellingCategories
            };
        }

        public async Task<List<CategoryHomePageDto>> GetCategoriesAsync()
        {
            var allCategories = (await _categoryRepository.GetAllAsync()).ToList();
            var options = new QueryOptions<Product>
            {
                Filter = p => p.ProductVariants.Any() 
            };
            var productsWithCategories = (await _productRepository.GetAllAsync(options))
                .Select(p => p.CategoryId)
                .Distinct()
                .ToHashSet();

            var dtoMap = allCategories.ToDictionary(
                c => c.CategoryId,
                c => new CategoryHomePageDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Slug = c.Slug,
                    Description = c.Description,
                    HasProduct = productsWithCategories.Contains(c.CategoryId)
                }
            );

            var roots = new List<CategoryHomePageDto>();

            foreach (var c in allCategories)
            {
                var dto = dtoMap[c.CategoryId];
                if (c.ParentId == null)
                {
                    roots.Add(dto);
                }
                else if (c.ParentId.HasValue && dtoMap.ContainsKey(c.ParentId.Value))
                {
                    dtoMap[c.ParentId.Value].Children.Add(dto);
                }
            }
            UpdateParentHasProduct(roots);
            return roots;
        }

        private void UpdateParentHasProduct(List<CategoryHomePageDto> categories)
        {
            foreach (var category in categories)
            {
                if (category.Children.Any())
                {
                    UpdateParentHasProduct(category.Children);
                    if (!category.HasProduct && category.Children.Any(c => c.HasProduct))
                    {
                        category.HasProduct = true;
                    }
                }
            }
        }

        public async Task<List<ProductSummaryDto>> GetBestSellingProductsAsync()
        {
            var since = DateTime.UtcNow.AddDays(-30);
            var options = new QueryOptions<Order>
            {
                Filter = o => o.CreatedAt >= since,
            };
            options.IncludeThen.Add(q => q.Include(o => o.OrderItems)
                                            .ThenInclude(oi => oi.ProductVariant)
                                                .ThenInclude(pv => pv!.Product)
                                                    .ThenInclude(p => p!.ProductImages));

            var orders = (await _orderRepository.GetAllAsync(options)).ToList();

            var grouped = orders
                .SelectMany(o => o.OrderItems)
                .Where(oi => oi.ProductVariant != null && oi.ProductVariant.Product != null)
                .GroupBy(oi => oi.ProductVariant!.Product!.ProductId)
                .Select(g =>
                {
                    var product = g.First().ProductVariant!.Product!;
                    var discount = product.DiscountPercentage;
                    var thumbnail = product.ProductImages.FirstOrDefault(pi => pi.IsPrimary)?.ImageUrl
                                    ?? product.ProductImages.OrderBy(pi => pi.DisplayOrder ?? 999).FirstOrDefault()?.ImageUrl;

                    return new ProductSummaryDto
                    {
                        ProductId = g.Key,
                        Name = product.Name,
                        Slug = product.Slug,
                        Thumbnail = thumbnail,
                        Price = product.BasePrice,
                        SalePrice = discount.HasValue
                                    ? product.BasePrice - (product.BasePrice * discount.Value / 100)
                                    : (decimal?)null,
                        SoldQuantity = g.Sum(oi => oi.Quantity),
                        DiscountPercentage = (int) (product.DiscountPercentage ?? 0m)

                    };
                })
                .OrderByDescending(p => p.SoldQuantity)
                .Take(10)
                .ToList();

            return grouped;
        }

        public async Task<List<ProductSummaryDto>> GetOnSaleProductsAsync()
        {
            var options = new QueryOptions<Product>
            {
                Filter = p => p.DiscountPercentage.HasValue && p.DiscountPercentage.Value >= 70
            };
            options.Includes.Add(p => p.ProductImages.Where(pi => pi.IsPrimary));
            var products = (await _productRepository.GetAllAsync(options)).ToList();
            var result = products
                .Select(p => new ProductSummaryDto
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Slug = p.Slug,
                    Thumbnail = p.ProductImages.FirstOrDefault()?.ImageUrl,
                    Price = p.BasePrice,
                    SalePrice = p.BasePrice - (p.BasePrice * (p.DiscountPercentage ?? 0) / 100),
                    SoldQuantity = 0,
                    DiscountPercentage = (int)(p.DiscountPercentage ?? 0m)
                })
                .OrderByDescending(p => p.DiscountPercentage)
                .ToList();

            return result;
        }

        public async Task<List<CategorySalesDto>> GetBestSellingCategoriesAsync()
        {
            var since = DateTime.UtcNow.AddDays(-30);
            
            var categoriesTree = await GetCategoriesAsync();
            
            var level3Categories = new List<(int CategoryId, string Name, string Slug)>();
            
            foreach (var level1 in categoriesTree)
            {
                foreach (var level2 in level1.Children)
                {
                    foreach (var level3 in level2.Children)
                    {
        
                        if (level3.Children.Count == 0)
                        {
                            level3Categories.Add((level3.CategoryId, level3.Name, level3.Slug));
                        }
                    }
                }
            }
            
            var level3CategoryIds = level3Categories.Select(c => c.CategoryId).ToHashSet();

            var options = new QueryOptions<Order>
            {
                Filter = o => o.CreatedAt >= since,
            };
            options.IncludeThen.Add(q => q.Include(o => o.OrderItems)
                                            .ThenInclude(oi => oi.ProductVariant)
                                                .ThenInclude(pv => pv!.Product));

            var orders = (await _orderRepository.GetAllAsync(options)).ToList();

            var categoryMap = level3Categories.ToDictionary(c => c.CategoryId, c => (c.Name, c.Slug));

            var categorySales = orders
                .SelectMany(o => o.OrderItems)
                .Where(oi => oi.ProductVariant?.Product != null 
                    && level3CategoryIds.Contains(oi.ProductVariant.Product.CategoryId))
                .GroupBy(oi => oi.ProductVariant!.Product!.CategoryId)
                .Select(g => new CategorySalesDto
                {
                    CategoryId = g.Key,
                    Name = categoryMap[g.Key].Name,
                    Slug = categoryMap[g.Key].Slug,
                    TotalSold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.Price)
                })
                .OrderByDescending(c => c.TotalSold)
                .Take(10)
                .ToList();

            return categorySales;
        }

    }
}
