using OnlineBookstore.Interfaces;
using OnlineBookstore.Models;
using OnlineBookstore.ViewModels;

namespace OnlineBookstore.Services
{
    public class BookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IReviewRepository _reviewRepository;

        public BookService(
            IBookRepository bookRepository,
            ICategoryRepository categoryRepository,
            IReviewRepository reviewRepository)
        {
            _bookRepository = bookRepository;
            _categoryRepository = categoryRepository;
            _reviewRepository = reviewRepository;
        }

        public async Task<BookListViewModel> GetBooksListAsync(
            string? searchTerm = null,
            string? author = null,
            int? categoryId = null,
            string? sortOrder = null,
            int page = 1,
            int pageSize = 12)
        {
            var books = await _bookRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();

            // Filter by search term
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                books = books.Where(b => b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by author
            if (!string.IsNullOrWhiteSpace(author))
            {
                books = books.Where(b => b.Author.Contains(author, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by category
            if (categoryId.HasValue)
            {
                books = books.Where(b => b.CategoryId == categoryId.Value);
            }

            // Sorting
            books = sortOrder?.ToLower() switch
            {
                "price_asc" => books.OrderBy(b => b.Price),
                "price_desc" => books.OrderByDescending(b => b.Price),
                "title_asc" => books.OrderBy(b => b.Title),
                "title_desc" => books.OrderByDescending(b => b.Title),
                "newest" => books.OrderByDescending(b => b.CreatedDate),
                _ => books.OrderByDescending(b => b.CreatedDate)
            };

            var totalCount = books.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var paginatedBooks = books
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var bookViewModels = new List<BookViewModel>();
            foreach (var book in paginatedBooks)
            {
                var avgRating = await _reviewRepository.GetAverageRatingAsync(book.BookId);
                bookViewModels.Add(new BookViewModel
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
                });
            }

            var categoryViewModels = categories.Select(c => new CategoryViewModel
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description
            }).ToList();

            return new BookListViewModel
            {
                Books = bookViewModels,
                Categories = categoryViewModels,
                SearchTerm = searchTerm,
                Author = author,
                CategoryId = categoryId,
                SortOrder = sortOrder,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<BookViewModel?> GetBookDetailsAsync(int id)
        {
            var book = await _bookRepository.GetByIdWithDetailsAsync(id);
            if (book == null) return null;

            return new BookViewModel
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
        }
    }
}
