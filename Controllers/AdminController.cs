using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Interfaces;
using OnlineBookstore.Models;
using OnlineBookstore.Services;
using OnlineBookstore.ViewModels;

namespace OnlineBookstore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly DashboardService _dashboardService;
        private readonly IUserRepository _userRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            DashboardService dashboardService,
            IUserRepository userRepository,
            IBookRepository bookRepository,
            IOrderRepository orderRepository,
            ICategoryRepository categoryRepository,
            UserManager<ApplicationUser> userManager)
        {
            _dashboardService = dashboardService;
            _userRepository = userRepository;
            _bookRepository = bookRepository;
            _orderRepository = orderRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var dashboard = await _dashboardService.GetDashboardStatisticsAsync();
            return View(dashboard);
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _userRepository.GetAllAsync();
            var viewModels = users.Select(u => new RecentUserViewModel
            {
                UserId = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? "",
                CreatedAt = u.CreatedAt
            }).ToList();

            return View(viewModels);
        }

        // GET: /Admin/Books
        public async Task<IActionResult> Books()
        {
            var books = await _bookRepository.GetAllAsync();
            var viewModels = books.Select(b => new BookViewModel
            {
                BookId = b.BookId,
                Title = b.Title,
                Author = b.Author,
                ISBN = b.ISBN,
                Price = b.Price,
                StockQuantity = b.StockQuantity,
                CategoryName = b.Category?.Name
            }).ToList();

            return View(viewModels);
        }

        // GET: /Admin/Orders
        public async Task<IActionResult> Orders()
        {
            var orders = await _orderRepository.GetAllAsync();
            return View(orders);
        }

        // POST: /Admin/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Invalid user ID.";
                return RedirectToAction(nameof(Users));
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Users));
            }

            // Prevent deleting yourself
            var currentUserId = _userManager.GetUserId(User);
            if (id == currentUserId)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Users));
            }

            await _userRepository.DeleteAsync(id);
            await _userRepository.SaveChangesAsync();

            TempData["Success"] = $"User '{user.FullName}' has been deleted successfully.";
            return RedirectToAction(nameof(Users));
        }
    }
}
