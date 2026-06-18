using OnlineBookstore.Models;

namespace OnlineBookstore.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book?> GetByIdAsync(int id);
        Task<Book?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Book>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm);
        Task<IEnumerable<Book>> SearchByAuthorAsync(string author);
        Task<IEnumerable<Book>> GetFeaturedBooksAsync();
        Task<IEnumerable<Book>> GetLatestBooksAsync();
        Task<IEnumerable<Book>> GetTopRatedBooksAsync();
        Task<int> GetTotalCountAsync();
        Task AddAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task SaveChangesAsync();
    }
}
