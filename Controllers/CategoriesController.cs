using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookstore.Interfaces;
using OnlineBookstore.Models;
using OnlineBookstore.ViewModels;

namespace OnlineBookstore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // GET: /Categories
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var viewModels = new List<CategoryViewModel>();

            foreach (var category in categories)
            {
                var bookCount = await _categoryRepository.GetBookCountAsync(category.CategoryId);
                viewModels.Add(new CategoryViewModel
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    Description = category.Description,
                    BookCount = bookCount
                });
            }

            return View(viewModels);
        }

        // GET: /Categories/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryRepository.GetByIdWithBooksAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var viewModel = new CategoryViewModel
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                BookCount = category.Books?.Count ?? 0
            };

            ViewBag.Books = category.Books?.Select(b => new BookViewModel
            {
                BookId = b.BookId,
                Title = b.Title,
                Author = b.Author,
                ISBN = b.ISBN,
                Price = b.Price,
                Description = b.Description,
                ImageUrl = b.ImageUrl,
                StockQuantity = b.StockQuantity,
                CategoryId = b.CategoryId
            }).ToList() ?? new List<BookViewModel>();

            return View(viewModel);
        }

        // GET: /Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var category = new Category
            {
                Name = model.Name,
                Description = model.Description
            };

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            TempData["Success"] = $"تم إنشاء التصنيف '{category.Name}' بنجاح!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Categories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var viewModel = new CategoryViewModel
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description
            };

            return View(viewModel);
        }

        // POST: /Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel model)
        {
            if (id != model.CategoryId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            category.Name = model.Name;
            category.Description = model.Description;

            await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveChangesAsync();

            TempData["Success"] = $"تم تحديث التصنيف '{category.Name}' بنجاح!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Categories/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var viewModel = new CategoryViewModel
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description
            };

            return View(viewModel);
        }

        // POST: /Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            if (await _categoryRepository.HasBooksAsync(id))
            {
                TempData["Error"] = $"لا يمكن حذف التصنيف '{category.Name}' لأنه يحتوي على كتب مرتبطة به.";
                return RedirectToAction(nameof(Index));
            }

            await _categoryRepository.DeleteAsync(id);
            await _categoryRepository.SaveChangesAsync();

            TempData["Success"] = $"تم حذف التصنيف '{category.Name}' بنجاح!";
            return RedirectToAction(nameof(Index));
        }
    }
}