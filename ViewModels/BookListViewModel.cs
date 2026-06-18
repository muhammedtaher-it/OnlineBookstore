namespace OnlineBookstore.ViewModels
{
    public class BookListViewModel
    {
        public List<BookViewModel> Books { get; set; } = new List<BookViewModel>();
        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
        public string? SearchTerm { get; set; }
        public string? Author { get; set; }
        public int? CategoryId { get; set; }
        public string? SortOrder { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 12;
        public int TotalCount { get; set; }
    }
}
