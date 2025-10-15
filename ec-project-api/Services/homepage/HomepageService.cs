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

            return new HomepageDto
            {
                Categories = categories,
                BestSellingProducts = bestSelling,
                OnSaleProducts = onSale
            };
        }

        public async Task<List<CategoryHomePageDto>> GetCategoriesAsync()
        {
            var allCategories = (await _categoryRepository.GetAllAsync()).ToList();

            var dtoMap = allCategories.ToDictionary(
                c => c.CategoryId,
                c => new CategoryHomePageDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Slug = c.Slug,
                    Description = c.Description
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

            return roots;
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
                                                .ThenInclude(pv => pv.Product)
                                                    .ThenInclude(p => p.ProductImages));

            var orders = (await _orderRepository.GetAllAsync(options)).ToList();

            var grouped = orders
                .SelectMany(o => o.OrderItems)
                .Where(oi => oi.ProductVariant != null && oi.ProductVariant.Product != null)
                .GroupBy(oi => oi.ProductVariant!.Product!.ProductId)
                .Select(g => new ProductSummaryDto
                {
                    ProductId = g.Key,
                    Name = g.First().ProductVariant!.Product!.Name,
                    Thumbnail = g.First().ProductVariant!.Product!.ProductImages.FirstOrDefault(pi => pi.IsPrimary)?.ImageUrl ?? g.First().ProductVariant!.Product!.ProductImages.OrderBy(pi => pi.DisplayOrder ?? 999).FirstOrDefault()?.ImageUrl,
                    Price = g.First().ProductVariant!.Product!.BasePrice,
                    SalePrice = g.First().ProductVariant!.Product!.DiscountPercentage.HasValue
                                ? g.First().ProductVariant!.Product!.BasePrice - (g.First().ProductVariant!.Product!.BasePrice * g.First().ProductVariant!.Product!.DiscountPercentage.Value / 100)
                                : (decimal?)null,
                    SoldQuantity = g.Sum(oi => oi.Quantity)
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
                Filter = p => p.DiscountPercentage.HasValue && p.DiscountPercentage.Value > 0
            };

            options.Includes.Add(p => p.ProductImages.Where(pi => pi.IsPrimary));

            var products = (await _productRepository.GetAllAsync(options)).ToList();

            var result = products.Select(p => new ProductSummaryDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Thumbnail = p.ProductImages.FirstOrDefault()?.ImageUrl,
                Price = p.BasePrice,
                SalePrice = p.DiscountPercentage.HasValue ? p.BasePrice - (p.BasePrice * p.DiscountPercentage.Value / 100) : (decimal?)null,
                SoldQuantity = 0
            })
            .OrderBy(p => p.SalePrice.HasValue ? p.SalePrice.Value : p.Price)
            .Take(10)
            .ToList();

            return result;
        }
    }
}
