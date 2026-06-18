using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.DTOs;
using OnlineBookstore.Interfaces;
using OnlineBookstore.Models;

namespace OnlineBookstore.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksApiController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IReviewRepository _reviewRepository;

        public BooksApiController(
            IBookRepository bookRepository,
            ICategoryRepository categoryRepository,
            IReviewRepository reviewRepository)
        {
            _bookRepository = bookRepository;
            _categoryRepository = categoryRepository;
            _reviewRepository = reviewRepository;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<List<BookDto>>>> GetBooks(
            [FromQuery] string? search = null,
            [FromQuery] string? author = null,
            [FromQuery] int? category = null,
            [FromQuery] string? sort = null)
        {
            IEnumerable<Book> books;

            if (!string.IsNullOrWhiteSpace(search))
            {
                books = await _bookRepository.SearchByTitleAsync(search);
            }
            else if (!string.IsNullOrWhiteSpace(author))
            {
                books = await _bookRepository.SearchByAuthorAsync(author);
            }
            else if (category.HasValue)
            {
                books = await _bookRepository.GetByCategoryAsync(category.Value);
            }
            else
            {
                books = await _bookRepository.GetAllAsync();
            }

            // Apply sorting
            books = sort?.ToLower() switch
            {
                "price_asc" => books.OrderBy(b => b.Price),
                "price_desc" => books.OrderByDescending(b => b.Price),
                "title_asc" => books.OrderBy(b => b.Title),
                "title_desc" => books.OrderByDescending(b => b.Title),
                _ => books
            };

            var bookDtos = new List<BookDto>();
            foreach (var book in books)
            {
                var avgRating = await _reviewRepository.GetAverageRatingAsync(book.BookId);
                var reviewCount = await _reviewRepository.GetReviewCountAsync(book.BookId);

                bookDtos.Add(new BookDto
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
                    CategoryName = book.Category?.Name,
                    AverageRating = avgRating,
                    ReviewCount = reviewCount,
                    CreatedDate = book.CreatedDate
                });
            }

            return Ok(new ApiResponseDto<List<BookDto>>
            {
                Success = true,
                Message = $"Retrieved {bookDtos.Count} books",
                Data = bookDtos
            });
        }

        // GET: api/books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<BookDto>>> GetBook(int id)
        {
            var book = await _bookRepository.GetByIdWithDetailsAsync(id);
            if (book == null)
            {
                return NotFound(new ApiResponseDto<BookDto>
                {
                    Success = false,
                    Message = $"Book with ID {id} not found"
                });
            }

            var avgRating = await _reviewRepository.GetAverageRatingAsync(book.BookId);
            var reviewCount = await _reviewRepository.GetReviewCountAsync(book.BookId);

            var bookDto = new BookDto
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
                CategoryName = book.Category?.Name,
                AverageRating = avgRating,
                ReviewCount = reviewCount,
                CreatedDate = book.CreatedDate
            };

            return Ok(new ApiResponseDto<BookDto>
            {
                Success = true,
                Message = "Book retrieved successfully",
                Data = bookDto
            });
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<BookDto>>> CreateBook([FromBody] CreateBookDto bookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDto<BookDto>
                {
                    Success = false,
                    Message = "Invalid data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            if (!await _categoryRepository.ExistsAsync(bookDto.CategoryId))
            {
                return BadRequest(new ApiResponseDto<BookDto>
                {
                    Success = false,
                    Message = $"Category with ID {bookDto.CategoryId} does not exist"
                });
            }

            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                ISBN = bookDto.ISBN,
                Price = bookDto.Price,
                Description = bookDto.Description,
                ImageUrl = bookDto.ImageUrl,
                StockQuantity = bookDto.StockQuantity,
                CategoryId = bookDto.CategoryId,
                CreatedDate = DateTime.UtcNow
            };

            await _bookRepository.AddAsync(book);
            await _bookRepository.SaveChangesAsync();

            var resultDto = new BookDto
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
                CategoryName = (await _categoryRepository.GetByIdAsync(book.CategoryId))?.Name,
                CreatedDate = book.CreatedDate
            };

            return CreatedAtAction(nameof(GetBook), new { id = book.BookId }, new ApiResponseDto<BookDto>
            {
                Success = true,
                Message = "Book created successfully",
                Data = resultDto
            });
        }

        // PUT: api/books/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<BookDto>>> UpdateBook(int id, [FromBody] UpdateBookDto bookDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDto<BookDto>
                {
                    Success = false,
                    Message = "Invalid data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var existingBook = await _bookRepository.GetByIdAsync(id);
            if (existingBook == null)
            {
                return NotFound(new ApiResponseDto<BookDto>
                {
                    Success = false,
                    Message = $"Book with ID {id} not found"
                });
            }

            if (!await _categoryRepository.ExistsAsync(bookDto.CategoryId))
            {
                return BadRequest(new ApiResponseDto<BookDto>
                {
                    Success = false,
                    Message = $"Category with ID {bookDto.CategoryId} does not exist"
                });
            }

            existingBook.Title = bookDto.Title;
            existingBook.Author = bookDto.Author;
            existingBook.ISBN = bookDto.ISBN;
            existingBook.Price = bookDto.Price;
            existingBook.Description = bookDto.Description;
            existingBook.ImageUrl = bookDto.ImageUrl;
            existingBook.StockQuantity = bookDto.StockQuantity;
            existingBook.CategoryId = bookDto.CategoryId;

            await _bookRepository.UpdateAsync(existingBook);
            await _bookRepository.SaveChangesAsync();

            var resultDto = new BookDto
            {
                BookId = existingBook.BookId,
                Title = existingBook.Title,
                Author = existingBook.Author,
                ISBN = existingBook.ISBN,
                Price = existingBook.Price,
                Description = existingBook.Description,
                ImageUrl = existingBook.ImageUrl,
                StockQuantity = existingBook.StockQuantity,
                CategoryId = existingBook.CategoryId,
                CategoryName = (await _categoryRepository.GetByIdAsync(existingBook.CategoryId))?.Name
            };

            return Ok(new ApiResponseDto<BookDto>
            {
                Success = true,
                Message = "Book updated successfully",
                Data = resultDto
            });
        }

        // DELETE: api/books/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto>> DeleteBook(int id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null)
            {
                return NotFound(new ApiResponseDto
                {
                    Success = false,
                    Message = $"Book with ID {id} not found"
                });
            }

            await _bookRepository.DeleteAsync(id);
            await _bookRepository.SaveChangesAsync();

            return Ok(new ApiResponseDto
            {
                Success = true,
                Message = $"Book '{book.Title}' deleted successfully"
            });
        }

        // GET: api/books/search/author/{author}
        [HttpGet("search/author/{author}")]
        public async Task<ActionResult<ApiResponseDto<List<BookDto>>>> SearchByAuthor(string author)
        {
            var books = await _bookRepository.SearchByAuthorAsync(author);
            var bookDtos = new List<BookDto>();

            foreach (var book in books)
            {
                var avgRating = await _reviewRepository.GetAverageRatingAsync(book.BookId);
                var reviewCount = await _reviewRepository.GetReviewCountAsync(book.BookId);

                bookDtos.Add(new BookDto
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
                    CategoryName = book.Category?.Name,
                    AverageRating = avgRating,
                    ReviewCount = reviewCount,
                    CreatedDate = book.CreatedDate
                });
            }

            return Ok(new ApiResponseDto<List<BookDto>>
            {
                Success = true,
                Message = $"Found {bookDtos.Count} books by author '{author}'",
                Data = bookDtos
            });
        }

        // GET: api/books/search/category/{categoryId}
        [HttpGet("search/category/{categoryId}")]
        public async Task<ActionResult<ApiResponseDto<List<BookDto>>>> SearchByCategory(int categoryId)
        {
            if (!await _categoryRepository.ExistsAsync(categoryId))
            {
                return NotFound(new ApiResponseDto<List<BookDto>>
                {
                    Success = false,
                    Message = $"Category with ID {categoryId} not found"
                });
            }

            var books = await _bookRepository.GetByCategoryAsync(categoryId);
            var bookDtos = new List<BookDto>();

            foreach (var book in books)
            {
                var avgRating = await _reviewRepository.GetAverageRatingAsync(book.BookId);
                var reviewCount = await _reviewRepository.GetReviewCountAsync(book.BookId);

                bookDtos.Add(new BookDto
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
                    CategoryName = book.Category?.Name,
                    AverageRating = avgRating,
                    ReviewCount = reviewCount,
                    CreatedDate = book.CreatedDate
                });
            }

            return Ok(new ApiResponseDto<List<BookDto>>
            {
                Success = true,
                Message = $"Found {bookDtos.Count} books in category",
                Data = bookDtos
            });
        }
    }
}
