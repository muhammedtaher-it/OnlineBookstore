using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.ViewModels
{
    public class BookViewModel
    {
        public int BookId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Author { get; set; } = string.Empty;

        [Required]
        [StringLength(13)]
        public string ISBN { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        [Range(0, 10000)]
        public int StockQuantity { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        [Required(ErrorMessage = "الرجاء اختيار التقييم")]
        [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون بين 1 و 5")]
        public int Rating { get; set; }
    }
}
