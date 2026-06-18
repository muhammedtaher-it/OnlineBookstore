using OnlineBookstore.Models;

namespace OnlineBookstore.Interfaces
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetAllAsync();
        Task<Review?> GetByIdAsync(int id);
        Task<IEnumerable<Review>> GetByBookIdAsync(int bookId);
        Task<IEnumerable<Review>> GetByUserIdAsync(string userId);
        Task<Review?> GetUserReviewForBookAsync(string userId, int bookId);
        Task<double> GetAverageRatingAsync(int bookId);
        Task<int> GetReviewCountAsync(int bookId);
        Task<bool> HasUserReviewedBookAsync(string userId, int bookId);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}
