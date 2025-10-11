using ec_project_api.Dtos.response;
using ec_project_api.Dtos.response.dashboard;
using ec_project_api.Interfaces.Orders;
using ec_project_api.Interfaces.Users;
using ec_project_api.Models;
using ec_project_api.Repository.Base;

namespace ec_project_api.Services.custom
{
    public class DashboardService : IDashboardService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        public DashboardService(IOrderRepository orderRepository, IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
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
    }
}
