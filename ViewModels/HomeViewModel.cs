namespace OnlineBookstore.ViewModels
{
    public class HomeViewModel
    {
        public List<BookViewModel> FeaturedBooks { get; set; } = new List<BookViewModel>();
        public List<BookViewModel> LatestBooks { get; set; } = new List<BookViewModel>();
        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
        public List<BookViewModel> TopRatedBooks { get; set; } = new List<BookViewModel>();
    }
}
