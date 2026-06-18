using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Services;

namespace OnlineBookstore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        // GET: /Cart
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _cartService.GetCartAsync(userId);
            return View(cart);
        }

        // POST: /Cart/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int bookId, int quantity = 1)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                await _cartService.AddToCartAsync(userId, bookId, quantity);
                TempData["Success"] = "Item added to cart successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to add item to cart: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                await _cartService.UpdateQuantityAsync(cartItemId, quantity);
                TempData["Success"] = "Cart updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to update cart: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                await _cartService.RemoveFromCartAsync(cartItemId);
                TempData["Success"] = "Item removed from cart successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to remove item: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/Clear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                await _cartService.ClearCartAsync(userId);
                TempData["Success"] = "Cart cleared successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to clear cart: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
