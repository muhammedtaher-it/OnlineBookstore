using OnlineBookstore.Models;

namespace OnlineBookstore.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Order>> GetRecentOrdersAsync(int count);
        Task<int> GetTotalOrdersCountAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetPendingOrdersCountAsync();
        Task<int> GetCompletedOrdersCountAsync();
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task UpdateStatusAsync(int orderId, string status);
        Task<bool> ExistsAsync(int id);
        Task SaveChangesAsync();
    }
}
