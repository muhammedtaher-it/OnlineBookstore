using OnlineBookstore.Models;

namespace OnlineBookstore.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category?> GetByIdWithBooksAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> HasBooksAsync(int id);
        Task<int> GetBookCountAsync(int categoryId);
        Task SaveChangesAsync();
    }
}
