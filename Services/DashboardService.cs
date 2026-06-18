using OnlineBookstore.Interfaces;
using OnlineBookstore.ViewModels;

namespace OnlineBookstore.Services
{
    public class DashboardService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IOrderRepository _orderRepository;

        public DashboardService(
            IUserRepository userRepository,
            IBookRepository bookRepository,
            IOrderRepository orderRepository)
        {
            _userRepository = userRepository;
            _bookRepository = bookRepository;
            _orderRepository = orderRepository;
        }

        public async Task<DashboardViewModel> GetDashboardStatisticsAsync()
        {
            var recentOrders = await _orderRepository.GetRecentOrdersAsync(10);
            var recentUsers = await _userRepository.GetRecentUsersAsync(10);

            var recentOrderViewModels = recentOrders.Select(o => new RecentOrderViewModel
            {
                OrderId = o.OrderId,
                CustomerName = o.User?.FullName ?? "Unknown",
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                OrderDate = o.OrderDate
            }).ToList();

            var recentUserViewModels = recentUsers.Select(u => new RecentUserViewModel
            {
                UserId = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? "",
                CreatedAt = u.CreatedAt
            }).ToList();

            return new DashboardViewModel
            {
                TotalUsers = await _userRepository.GetTotalUsersCountAsync(),
                TotalBooks = await _bookRepository.GetTotalCountAsync(),
                TotalOrders = await _orderRepository.GetTotalOrdersCountAsync(),
                TotalRevenue = await _orderRepository.GetTotalRevenueAsync(),
                PendingOrders = await _orderRepository.GetPendingOrdersCountAsync(),
                CompletedOrders = await _orderRepository.GetCompletedOrdersCountAsync(),
                RecentOrders = recentOrderViewModels,
                RecentUsers = recentUserViewModels
            };
        }
    }
}
