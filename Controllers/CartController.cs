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
                TempData["Success"] = "تمت إضافة العنصر إلى السلة بنجاح!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"فشل إضافة العنصر إلى السلة: {ex.Message}";
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
                TempData["Success"] = "تم تحديث السلة بنجاح!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"فشل تحديث السلة: {ex.Message}";
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
                TempData["Success"] = "تمت إزالة العنصر من السلة بنجاح!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"فشل إزالة العنصر: {ex.Message}";
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
                TempData["Success"] = "تم تفريغ السلة بنجاح!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"فشل تفريغ السلة: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}