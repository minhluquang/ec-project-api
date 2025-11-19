
using ec_project_api.Constants.variables;
using ec_project_api.Dtos.response.dashboard;
using ec_project_api.Interfaces.Orders;
using ec_project_api.Interfaces.Users;
using ec_project_api.Models;
using ec_project_api.Repository.Base;
using System.Linq.Expressions;
using ec_project_api.Interfaces.PurchaseOrders;

namespace ec_project_api.Services.custom
{
    public class DashboardService : IDashboardService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;

        public DashboardService(IOrderRepository orderRepository, IUserRepository userRepository, IPurchaseOrderRepository purchaseOrderRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
        }

        public async Task<DashboardOverviewDto> GetOverviewAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfLastMonth = startOfMonth.AddMonths(-1);

            // Get all orders for current and last month
            var ordersOptions = new QueryOptions<Order>
            {
                Filter = o => o.CreatedAt >= startOfLastMonth,
                Includes = new List<System.Linq.Expressions.Expression<Func<Order, object>>>
                {
                    o => o.OrderItems
                }
            };

            var orders = (await _orderRepository.GetAllAsync(ordersOptions)).ToList();
            
            var currentMonthOrders = orders.Where(o => o.CreatedAt >= startOfMonth).ToList();
            var lastMonthOrders = orders.Where(o => o.CreatedAt >= startOfLastMonth && o.CreatedAt < startOfMonth).ToList();

            // Calculate monthly revenue
            var monthlyRevenue = currentMonthOrders.Sum(o => o.TotalAmount);
            var lastMonthRevenue = lastMonthOrders.Sum(o => o.TotalAmount);
            var revenueChangePercent = lastMonthRevenue > 0 
                ? ((monthlyRevenue - lastMonthRevenue) / lastMonthRevenue) * 100 
                : 0;

            // Calculate total orders
            var totalOrders = currentMonthOrders.Count;
            var lastMonthOrderCount = lastMonthOrders.Count;
            var orderChangePercent = lastMonthOrderCount > 0 
                ? ((decimal)(totalOrders - lastMonthOrderCount) / lastMonthOrderCount) * 100 
                : 0;

            // Calculate new customers
            var usersOptions = new QueryOptions<User>
            {
                Filter = u => u.CreatedAt >= startOfLastMonth
            };
            var users = (await _userRepository.GetAllAsync(usersOptions)).ToList();
            
            var newCustomers = users.Count(u => u.CreatedAt >= startOfMonth);
            var lastMonthCustomers = users.Count(u => u.CreatedAt >= startOfLastMonth && u.CreatedAt < startOfMonth);
            var customerChangePercent = lastMonthCustomers > 0 
                ? ((decimal)(newCustomers - lastMonthCustomers) / lastMonthCustomers) * 100 
                : 0;

            // Calculate products sold (total quantity from order items)
            var productsSold = currentMonthOrders.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity);
            var lastMonthProductsSold = lastMonthOrders.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity);
            var productChangePercent = lastMonthProductsSold > 0 
                ? ((decimal)(productsSold - lastMonthProductsSold) / lastMonthProductsSold) * 100 
                : 0;

            return new DashboardOverviewDto
            {
                MonthlyRevenue = monthlyRevenue,
                RevenueChangePercent = Math.Round(revenueChangePercent, 1),
                TotalOrders = totalOrders,
                OrderChangePercent = Math.Round(orderChangePercent, 1),
                NewCustomers = newCustomers,
                CustomerChangePercent = Math.Round(customerChangePercent, 1),
                ProductsSold = productsSold,
                ProductChangePercent = Math.Round(productChangePercent, 1)
            };
        }

        public async Task<MonthlyRevenueResponse> GetMonthlyRevenueAsync(string timeRange)
        {
            var now = DateTime.UtcNow;
            DateTime startDate;
            DateTime endDate = now;
            List<MonthlyRevenueDto> revenueData = new();

            switch (timeRange.ToLower())
            {
                case "current_month":
                    // Tháng này - hiển thị theo ngày
                    startDate = new DateTime(now.Year, now.Month, 1);
                    revenueData = await GetDailyRevenueInMonth(startDate, endDate);
                    break;

                case "last_month":
                    // Tháng trước - hiển thị theo ngày
                    startDate = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
                    endDate = new DateTime(now.Year, now.Month, 1).AddDays(-1);
                    revenueData = await GetDailyRevenueInMonth(startDate, endDate);
                    break;

                case "last_3_months":
                    // 3 tháng gần nhất - hiển thị theo tháng
                    startDate = new DateTime(now.Year, now.Month, 1).AddMonths(-2);
                    revenueData = await GetMonthlyRevenue(startDate, endDate);
                    break;

                case "current_year":
                    // Năm nay - hiển thị theo tháng (12 tháng)
                    startDate = new DateTime(now.Year, 1, 1);
                    revenueData = await GetMonthlyRevenue(startDate, endDate);
                    break;

                default:
                    throw new ArgumentException("Invalid time range. Use: current_month, last_month, last_3_months, or current_year");
            }

            var totalRevenue = revenueData.Sum(d => d.Revenue);
            var totalOrders = revenueData.Sum(d => d.OrderCount);

            return new MonthlyRevenueResponse
            {
                TimeRange = timeRange,
                Data = revenueData,
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders
            };
        }

        private async Task<List<MonthlyRevenueDto>> GetDailyRevenueInMonth(DateTime startDate, DateTime endDate)
        {
            var options = new QueryOptions<Order>
            {
                Filter = o => o.CreatedAt >= startDate && o.CreatedAt <= endDate
            };

            var orders = (await _orderRepository.GetAllAsync(options)).ToList();

            var dailyRevenue = orders
                .GroupBy(o => o.CreatedAt.Day)
                .Select(g => new MonthlyRevenueDto
                {
                    Period = $"Ngày {g.Key}",
                    Revenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(d => int.Parse(d.Period.Replace("Ngày ", "")))
                .ToList();

            return dailyRevenue;
        }

        private async Task<List<MonthlyRevenueDto>> GetMonthlyRevenue(DateTime startDate, DateTime endDate)
        {
            var options = new QueryOptions<Order>
            {
                Filter = o => o.CreatedAt >= startDate && o.CreatedAt <= endDate
            };

            var orders = (await _orderRepository.GetAllAsync(options)).ToList();

            var monthlyRevenue = orders
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new MonthlyRevenueDto
                {
                    Period = $"T{g.Key.Month}",
                    Revenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(d => int.Parse(d.Period.Replace("T", "")))
                .ToList();

            // Fill missing months with zero revenue
            var result = new List<MonthlyRevenueDto>();
            var current = startDate;
            
            while (current <= endDate)
            {
                var monthData = monthlyRevenue.FirstOrDefault(m => m.Period == $"T{current.Month}");
                if (monthData != null)
                {
                    result.Add(monthData);
                }
                else
                {
                    result.Add(new MonthlyRevenueDto
                    {
                        Period = $"T{current.Month}",
                        Revenue = 0,
                        OrderCount = 0
                    });
                }
                current = current.AddMonths(1);
            }

            return result;
        }
        
        public async Task<List<CategorySalesPercentageDto>> GetCategorySalesPercentageAsync(
            DateTime? startDate,
            DateTime? endDate,
            string? preset)
        {
            // 1. Xử lý preset nếu có
            if (!string.IsNullOrEmpty(preset))
            {
                var today = DateTime.Now.Date;
        
                switch (preset)
                {
                    case "this-month":
                        startDate = new DateTime(today.Year, today.Month, 1);
                        endDate = today;
                        break;
        
                    case "last-month":
                        var lastMonth = today.AddMonths(-1);
                        startDate = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                        endDate = new DateTime(lastMonth.Year, lastMonth.Month, DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month));
                        break;
        
                    case "last-7-days":
                        startDate = today.AddDays(-7);
                        endDate = today;
                        break;
        
                    case "this-year":
                        startDate = new DateTime(today.Year, 1, 1);
                        endDate = today;
                        break;
                }
            }
            
            if (startDate != null && endDate != null)
            {
                endDate = endDate.Value.AddDays(1);  
            }

            // 2. Build query options - Include từng navigation property riêng biệt
            var options = new QueryOptions<Order>
            {
                Filter = o => 
                    o.Status.Name == StatusVariables.Delivered &&
                    o.DeliveryAt.HasValue &&
                    o.DeliveryAt.Value >= startDate &&
                    o.DeliveryAt.Value < endDate,
                Includes = new List<Expression<Func<Order, object>>>
                {
                    o => o.OrderItems
                }
            };
        
            // 3. Lấy dữ liệu
            var orders = (await _orderRepository.GetAllAsync(options)).ToList();
        
            // 4. Group theo danh mục cấp 2
            var categorySales = orders
                .SelectMany(o => o.OrderItems)
                .Where(oi => oi.ProductVariant?.Product?.Category != null)
                .Select(oi => new
                {
                    OrderItem = oi,
                    Category = oi.ProductVariant.Product.Category,
                    Level2Category = oi.ProductVariant.Product.Category.Parent ?? oi.ProductVariant.Product.Category
                })
                .Where(x => x.Level2Category.ParentId != null) // Chỉ lấy category cấp 2 (có ParentId)
                .GroupBy(x => x.Level2Category.Name)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    TotalSales = g.Sum(x => x.OrderItem.Quantity * x.OrderItem.Price),
                    TotalQuantity = g.Sum(x => x.OrderItem.Quantity)
                })
                .ToList();
        
            var totalRevenue = categorySales.Sum(cs => cs.TotalSales);
        
            // 5. Map sang DTO + tính %
            var result = categorySales
                .Select(cs => new CategorySalesPercentageDto
                {
                    CategoryName = cs.CategoryName,
                    TotalSales = cs.TotalSales,
                    Percentage = totalRevenue > 0
                        ? Math.Round((decimal)((cs.TotalSales / totalRevenue) * 100), 1)
                        : 0
                })
                .OrderByDescending(x => x.Percentage)
                .ToList();
        
            return result;
        }
        
        public async Task<List<MonthlyRevenueStatsDto>> GetMonthlyRevenueStatsAsync(int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year + 1, 1, 1);
        
            var options = new QueryOptions<Order>
            {
                Filter = o => 
                    o.Status.Name == StatusVariables.Delivered &&
                    o.DeliveryAt.HasValue &&
                    o.DeliveryAt.Value >= startDate &&
                    o.DeliveryAt.Value < endDate
            };
        
            var orders = (await _orderRepository.GetAllAsync(options)).ToList();
        
            // Group theo tháng giao hàng
            var monthlyData = orders
                .Where(o => o.DeliveryAt.HasValue)
                .GroupBy(o => o.DeliveryAt.Value.Month)
                .Select(g => new MonthlyRevenueStatsDto
                {
                    Period = $"T{g.Key}",
                    Revenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .ToList();
        
            // Tạo đủ 12 tháng, nếu tháng nào không có đơn thì trả 0
            var result = Enumerable.Range(1, 12)
                .Select(month => monthlyData.FirstOrDefault(m => m.Period == $"T{month}")
                                 ?? new MonthlyRevenueStatsDto
                                 {
                                     Period = $"T{month}",
                                     Revenue = 0,
                                     OrderCount = 0
                                 })
                .ToList();
        
            return result;
        }

        public async Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(int top, int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year + 1, 1, 1);

            var options = new QueryOptions<Order>
            {
                Filter = o =>
                    o.Status.Name == StatusVariables.Delivered &&
                    o.DeliveryAt.HasValue &&
                    o.DeliveryAt.Value >= startDate &&
                    o.DeliveryAt.Value < endDate,
                Includes = new List<Expression<Func<Order, object>>>
                {
                    o => o.OrderItems
                }
            };

            var orders = (await _orderRepository.GetAllAsync(options)).ToList();

            var topProducts = orders
                .SelectMany(o => o.OrderItems)
                .Where(oi => oi.ProductVariant?.Product != null)
                .GroupBy(oi => new
                {
                    Product = oi.ProductVariant?.Product,
                    Category = oi.ProductVariant?.Product.Category
                })
                .Select(g => new TopSellingProductDto
                {
                    ProductId = g.Key.Product.ProductId,
                    ProductName = g.Key.Product.Name,
                    ProductImage = g.Key.Product.ProductImages.FirstOrDefault(pi => pi.IsPrimary)?.ImageUrl,
                    CategoryLv2Name = g.Key.Category?.Parent?.Name,
                    TotalQuantitySold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.Price)
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .ThenByDescending(p => p.TotalRevenue)
                .Take(top)
                .ToList();

            return topProducts;
        }

        public async Task<List<DailySalesDto>> GetWeeklySalesAsync()
        {
            var today = DateTime.UtcNow.Date;

            // Tính ngày đầu tuần (Thứ 2)
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
            if (today.DayOfWeek == DayOfWeek.Sunday)
                startOfWeek = startOfWeek.AddDays(-7);

            // Tính ngày cuối tuần (Chủ nhật)
            var endOfWeek = startOfWeek.AddDays(6);

            var options = new QueryOptions<Order>
            {
                Filter = o =>
                    o.Status.Name == StatusVariables.Delivered &&
                    o.DeliveryAt.HasValue &&
                    o.DeliveryAt.Value >= startOfWeek &&
                    o.DeliveryAt.Value <= endOfWeek,
                Includes = new List<Expression<Func<Order, object>>>
                {
                    o => o.OrderItems
                }
            };

            var orders = (await _orderRepository.GetAllAsync(options)).ToList();

            var dailySales = new List<DailySalesDto>();

            for (int i = 0; i < 7; i++)
            {
                var currentDate = startOfWeek.AddDays(i);
                var dayOrders = orders.Where(o => o.DeliveryAt.Value.Date == currentDate).ToList();

                dailySales.Add(new DailySalesDto
                {
                    Date = currentDate,
                    DayOfWeek = currentDate.ToString("dddd", new System.Globalization.CultureInfo("vi-VN")),
                    Revenue = dayOrders.Sum(o => o.TotalAmount),
                    OrderCount = dayOrders.Count,
                    ProductsSold = dayOrders.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity)
                });
            }

            return dailySales;
        }
        
        public async Task<List<MonthlyProfitDto>> GetMonthlyProfitAsync(int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year + 1, 1, 1);
        
            // Lấy orders delivered
            var orderOptions = new QueryOptions<Order>
            {
                Filter = o =>
                    o.Status.Name == StatusVariables.Delivered &&
                    o.DeliveryAt.HasValue &&
                    o.DeliveryAt.Value >= startDate &&
                    o.DeliveryAt.Value < endDate
            };
        
            var orders = (await _orderRepository.GetAllAsync(orderOptions)).ToList();
        
            // Lấy orderPurchase 
            var purchaseOptions = new QueryOptions<PurchaseOrder>
            {
                Filter = op =>
                    op.Status.Name == StatusVariables.Completed &&
                    op.UpdatedAt >= startDate &&
                    op.UpdatedAt < endDate
            };
        
            var purchases = (await _purchaseOrderRepository.GetAllAsync(purchaseOptions)).ToList();
        
            // Tính theo từng tháng
            var monthlyProfit = Enumerable.Range(1, 12).Select(month =>
            {
                var monthOrders = orders.Where(o => o.DeliveryAt.Value.Month == month).ToList();
                var monthPurchases = purchases.Where(p => p.CreatedAt.Month == month).ToList();
        
                // Tính revenue: nếu IsShip = true thì trừ ShippingFee
                var totalRevenue = monthOrders.Sum(o =>
                    o.IsFreeShip ? o.TotalAmount - o.ShippingFee : o.TotalAmount);
        
                // Tính shipping revenue (tổng phí ship thu được)
                var shippingRevenue = monthOrders
                    .Where(o => o.IsFreeShip)
                    .Sum(o => o.ShippingFee);
        
                // Tính cost từ orderPurchase
                var totalCost = monthPurchases.Sum(p => p.TotalAmount);
        
                // Tính profit
                var profit = totalRevenue - totalCost;
                var profitMargin = totalRevenue > 0 ? (profit / totalRevenue) * 100 : 0;
        
                return new MonthlyProfitDto
                {
                    Period = $"T{month}",
                    TotalRevenue = totalRevenue,
                    TotalCost = totalCost,
                    ShippingRevenue = shippingRevenue,
                    Profit = profit,
                    ProfitMargin = Math.Round(profitMargin, 1)
                };
            }).ToList();
        
            return monthlyProfit;
        }
    }
}
