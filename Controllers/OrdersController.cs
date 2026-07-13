using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Interfaces;
using OnlineBookstore.Services;
using OnlineBookstore.ViewModels;

namespace OnlineBookstore.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly OrderService _orderService;
        private readonly IOrderRepository _orderRepository;
        private readonly CartService _cartService;

        public OrdersController(
            OrderService orderService,
            IOrderRepository orderRepository,
            CartService cartService)
        {
            _orderService = orderService;
            _orderRepository = orderRepository;
            _cartService = cartService;
        }

        // GET: /Orders
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _orderService.GetUserOrdersAsync(userId);
            return View(orders);
        }

        // GET: /Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // Verify the order belongs to the current user (unless admin)
            if (!User.IsInRole("Admin"))
            {
                var orderEntity = await _orderRepository.GetByIdAsync(id);
                if (orderEntity?.UserId != userId)
                {
                    return Forbid();
                }
            }

            return View(order);
        }

        // GET: /Orders/Checkout
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _cartService.GetCartAsync(userId);
            if (!cart.CartItems.Any())
            {
                TempData["Error"] = "سلتك فارغة. أضف بعض العناصر قبل إتمام الشراء.";
                return RedirectToAction("Index", "Cart");
            }

            var viewModel = new CheckoutViewModel
            {
                Cart = cart
            };

            return View(viewModel);
        }

        // POST: /Orders/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                model.Cart = await _cartService.GetCartAsync(userId);
                return View(model);
            }

            try
            {
                var order = await _orderService.CreateOrderAsync(userId, model);
                TempData["Success"] = "تم تأكيد الطلب بنجاح!";
                return RedirectToAction("Confirmation", new { id = order.OrderId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                model.Cart = await _cartService.GetCartAsync(userId);
                return View(model);
            }
        }

        // GET: /Orders/Confirmation/5
        public async Task<IActionResult> Confirmation(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: /Orders/Manage (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Manage()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        // POST: /Orders/UpdateStatus (Admin only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, string status)
        {
            var success = await _orderService.UpdateOrderStatusAsync(orderId, status);
            if (!success)
            {
                TempData["Error"] = "الطلب غير موجود.";
                return RedirectToAction(nameof(Manage));
            }

            // ترجمة حالة الطلب للعرض في رسالة النجاح للمشرف
            var statusAr = status switch
            {
                "Pending" => "قيد الانتظار",
                "Processing" => "قيد المعالجة",
                "Completed" => "مكتمل",
                "Delivered" => "تم التوصيل",
                "Cancelled" => "ملغي",
                _ => status // إذا كانت حالة أخرى، اتركها كما هي
            };

            TempData["Success"] = $"تم تحديث حالة الطلب رقم #{orderId} إلى '{statusAr}'.";
            return RedirectToAction(nameof(Manage));
        }
    }
}