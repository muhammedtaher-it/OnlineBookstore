using System.ComponentModel.DataAnnotations;

namespace OnlineBookstore.DTOs
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public int BookId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateReviewDto
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }
    }

    public class BookRatingDto
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }
}
