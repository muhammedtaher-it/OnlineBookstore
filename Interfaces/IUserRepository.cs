using OnlineBookstore.Models;

namespace OnlineBookstore.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<IEnumerable<ApplicationUser>> GetRecentUsersAsync(int count);
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetTotalAdminsCountAsync();
        Task UpdateAsync(ApplicationUser user);
        Task DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task SaveChangesAsync();
    }
}
