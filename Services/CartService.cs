using OnlineBookstore.Interfaces;
using OnlineBookstore.Models;
using OnlineBookstore.ViewModels;

namespace OnlineBookstore.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IBookRepository _bookRepository;

        public CartService(ICartRepository cartRepository, IBookRepository bookRepository)
        {
            _cartRepository = cartRepository;
            _bookRepository = bookRepository;
        }

        public async Task<CartViewModel> GetCartAsync(string userId)
        {
            var cartItems = await _cartRepository.GetCartByUserIdAsync(userId);
            var cartItemViewModels = new List<CartItemViewModel>();

            foreach (var item in cartItems)
            {
                cartItemViewModels.Add(new CartItemViewModel
                {
                    CartItemId = item.CartItemId,
                    BookId = item.BookId,
                    Title = item.Book.Title,
                    Author = item.Book.Author,
                    ImageUrl = item.Book.ImageUrl,
                    Price = item.Book.Price,
                    Quantity = item.Quantity
                });
            }

            var totalPrice = await _cartRepository.GetCartTotalAsync(userId);
            var totalItems = await _cartRepository.GetCartItemCountAsync(userId);

            return new CartViewModel
            {
                CartItems = cartItemViewModels,
                TotalPrice = totalPrice,
                TotalItems = totalItems
            };
        }

        public async Task AddToCartAsync(string userId, int bookId, int quantity = 1)
        {
            var existingItem = await _cartRepository.GetCartItemAsync(userId, bookId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                await _cartRepository.UpdateAsync(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId,
                    BookId = bookId,
                    Quantity = quantity
                };
                await _cartRepository.AddAsync(cartItem);
            }

            await _cartRepository.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(int cartItemId, int quantity)
        {
            var cartItem = await _cartRepository.GetByIdAsync(cartItemId);
            if (cartItem != null)
            {
                if (quantity <= 0)
                {
                    await _cartRepository.RemoveAsync(cartItemId);
                }
                else
                {
                    cartItem.Quantity = quantity;
                    await _cartRepository.UpdateAsync(cartItem);
                }
                await _cartRepository.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCartAsync(int cartItemId)
        {
            await _cartRepository.RemoveAsync(cartItemId);
            await _cartRepository.SaveChangesAsync();
        }

        public async Task ClearCartAsync(string userId)
        {
            await _cartRepository.ClearCartAsync(userId);
            await _cartRepository.SaveChangesAsync();
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            return await _cartRepository.GetCartItemCountAsync(userId);
        }
    }
}
