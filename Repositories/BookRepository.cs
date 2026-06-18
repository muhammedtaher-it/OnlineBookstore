using Microsoft.EntityFrameworkCore;
using OnlineBookstore.Data;
using OnlineBookstore.Interfaces;
using OnlineBookstore.Models;

namespace OnlineBookstore.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books
                .Include(b => b.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookId == id);
        }

        public async Task<Book?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Reviews)
                .ThenInclude(r => r.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookId == id);
        }

        public async Task<IEnumerable<Book>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Books
                .Include(b => b.Category)
                .Where(b => b.CategoryId == categoryId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm)
        {
            return await _context.Books
                .Include(b => b.Category)
                .Where(b => b.Title.Contains(searchTerm))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> SearchByAuthorAsync(string author)
        {
            return await _context.Books
                .Include(b => b.Category)
                .Where(b => b.Author.Contains(author))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetFeaturedBooksAsync()
        {
            return await _context.Books
                .Include(b => b.Category)
                .OrderByDescending(b => b.Reviews.Count)
                .Take(6)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetLatestBooksAsync()
        {
            return await _context.Books
                .Include(b => b.Category)
                .OrderByDescending(b => b.CreatedDate)
                .Take(8)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetTopRatedBooksAsync()
        {
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Reviews)
                .OrderByDescending(b => b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0)
                .Take(4)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Books.CountAsync();
        }

        public async Task AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
        }

        public async Task UpdateAsync(Book book)
        {
            _context.Books.Update(book);
        }

        public async Task DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Books.AnyAsync(b => b.BookId == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
