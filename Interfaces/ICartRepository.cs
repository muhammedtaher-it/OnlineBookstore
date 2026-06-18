using OnlineBookstore.Models;

namespace OnlineBookstore.Interfaces
{
    public interface ICartRepository
    {
        Task<IEnumerable<CartItem>> GetCartByUserIdAsync(string userId);
        Task<CartItem?> GetCartItemAsync(string userId, int bookId);
        Task<CartItem?> GetByIdAsync(int cartItemId);
        Task AddAsync(CartItem cartItem);
        Task UpdateAsync(CartItem cartItem);
        Task RemoveAsync(int cartItemId);
        Task ClearCartAsync(string userId);
        Task<int> GetCartItemCountAsync(string userId);
        Task<decimal> GetCartTotalAsync(string userId);
        Task SaveChangesAsync();
    }
}
