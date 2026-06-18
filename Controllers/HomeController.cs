using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Interfaces;
using OnlineBookstore.ViewModels;

namespace OnlineBookstore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IReviewRepository _reviewRepository;

        public HomeController(
            IBookRepository bookRepository,
            ICategoryRepository categoryRepository,
            IReviewRepository reviewRepository)
        {
            _bookRepository = bookRepository;
            _categoryRepository = categoryRepository;
            _reviewRepository = reviewRepository;
        }

        public async Task<IActionResult> Index()
        {
            var featuredBooks = await _bookRepository.GetFeaturedBooksAsync();
            var latestBooks = await _bookRepository.GetLatestBooksAsync();
            var topRatedBooks = await _bookRepository.GetTopRatedBooksAsync();
            var categories = await _categoryRepository.GetAllAsync();

            var viewModel = new HomeViewModel
            {
                FeaturedBooks = featuredBooks.Select(b => new BookViewModel
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    Price = b.Price,
                    Description = b.Description,
                    ImageUrl = b.ImageUrl,
                    StockQuantity = b.StockQuantity,
                    CategoryId = b.CategoryId,
                    CategoryName = b.Category?.Name
                }).ToList(),
                LatestBooks = latestBooks.Select(b => new BookViewModel
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    Price = b.Price,
                    Description = b.Description,
                    ImageUrl = b.ImageUrl,
                    StockQuantity = b.StockQuantity,
                    CategoryId = b.CategoryId,
                    CategoryName = b.Category?.Name
                }).ToList(),
                TopRatedBooks = topRatedBooks.Select(b => new BookViewModel
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Author = b.Author,
                    ISBN = b.ISBN,
                    Price = b.Price,
                    Description = b.Description,
                    ImageUrl = b.ImageUrl,
                    StockQuantity = b.StockQuantity,
                    CategoryId = b.CategoryId,
                    CategoryName = b.Category?.Name
                }).ToList(),
                Categories = categories.Select(c => new CategoryViewModel
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description
                }).ToList()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
