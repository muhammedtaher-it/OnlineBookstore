using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineBookstore.Interfaces;
using OnlineBookstore.Models;
using OnlineBookstore.Services;
using OnlineBookstore.ViewModels;

namespace OnlineBookstore.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly BookService _bookService;
        private readonly ReviewService _reviewService;

        public BooksController(
            IBookRepository bookRepository,
            ICategoryRepository categoryRepository,
            IReviewRepository reviewRepository,
            BookService bookService,
            ReviewService reviewService)
        {
            _bookRepository = bookRepository;
            _categoryRepository = categoryRepository;
            _reviewRepository = reviewRepository;
            _bookService = bookService;
            _reviewService = reviewService;
        }

        // GET: /Books
        public async Task<IActionResult> Index(
            string? search = null,
            string? author = null,
            int? category = null,
            string? sort = null,
            int page = 1)
        {
            var viewModel = await _bookService.GetBooksListAsync(search, author, category, sort, page, 12);
            return View(viewModel);
        }

        // GET: /Books/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookRepository.GetByIdWithDetailsAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var bookViewModel = new BookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Price = book.Price,
                Description = book.Description,
                ImageUrl = book.ImageUrl,
                StockQuantity = book.StockQuantity,
                CategoryId = book.CategoryId,
                CategoryName = book.Category?.Name
            };

            var reviews = await _reviewService.GetBookReviewsAsync(id);
            var rating = await _reviewService.GetBookRatingAsync(id);

            ViewBag.Reviews = reviews;
            ViewBag.Rating = rating;
            ViewBag.UserHasReviewed = false;

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _bookRepository.GetType().GetMethod("GetByIdAsync") == null ? "" : User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
                if (!string.IsNullOrEmpty(userId))
                {
                    ViewBag.UserHasReviewed = await _reviewService.HasUserReviewedBookAsync(userId, id);
                }
            }

            return View(bookViewModel);
        }

        // POST: /Books/AddReview
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(CreateReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", new { id = model.BookId });
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                await _reviewService.CreateReviewAsync(userId, model.BookId, model.Rating, model.Comment);
                TempData["Success"] = "Your review has been submitted successfully!";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", new { id = model.BookId });
        }

        // GET: /Books/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
            return View();
        }

        // POST: /Books/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "CategoryId", "Name", model.CategoryId);
                return View(model);
            }

            var book = new Book
            {
                Title = model.Title,
                Author = model.Author,
                ISBN = model.ISBN,
                Price = model.Price,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                StockQuantity = model.StockQuantity,
                CategoryId = model.CategoryId,
                CreatedDate = DateTime.UtcNow
            };

            await _bookRepository.AddAsync(book);
            await _bookRepository.SaveChangesAsync();

            TempData["Success"] = $"Book '{book.Title}' has been created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Books/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var viewModel = new BookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Price = book.Price,
                Description = book.Description,
                ImageUrl = book.ImageUrl,
                StockQuantity = book.StockQuantity,
                CategoryId = book.CategoryId,
                CategoryName = book.Category?.Name
            };

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name", book.CategoryId);
            return View(viewModel);
        }

        // POST: /Books/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookViewModel model)
        {
            if (id != model.BookId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "CategoryId", "Name", model.CategoryId);
                return View(model);
            }

            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            // Update book properties
            book.Title = model.Title;
            book.Author = model.Author;
            book.ISBN = model.ISBN;
            book.Price = model.Price;
            book.Description = model.Description;
            book.ImageUrl = model.ImageUrl;
            book.StockQuantity = model.StockQuantity;
            book.CategoryId = model.CategoryId;

            await _bookRepository.UpdateAsync(book);
            await _bookRepository.SaveChangesAsync();

            TempData["Success"] = $"Book '{book.Title}' has been updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Books/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var viewModel = new BookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Price = book.Price,
                Description = book.Description,
                ImageUrl = book.ImageUrl,
                StockQuantity = book.StockQuantity,
                CategoryName = book.Category?.Name
            };

            return View(viewModel);
        }

        // POST: /Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            await _bookRepository.DeleteAsync(id);
            await _bookRepository.SaveChangesAsync();

            TempData["Success"] = $"Book '{book.Title}' has been deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
