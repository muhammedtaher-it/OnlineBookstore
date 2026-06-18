using OnlineBookstore.Interfaces;
using OnlineBookstore.Models;
using OnlineBookstore.ViewModels;

namespace OnlineBookstore.Services
{
    public class ReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;

        public ReviewService(IReviewRepository reviewRepository, IUserRepository userRepository)
        {
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
        }

        public async Task<Review> CreateReviewAsync(string userId, int bookId, int rating, string? comment)
        {
            if (await _reviewRepository.HasUserReviewedBookAsync(userId, bookId))
                throw new InvalidOperationException("You have already reviewed this book.");

            var review = new Review
            {
                UserId = userId,
                BookId = bookId,
                Rating = rating,
                Comment = comment
            };

            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();

            return review;
        }

        public async Task<List<ReviewViewModel>> GetBookReviewsAsync(int bookId)
        {
            var reviews = await _reviewRepository.GetByBookIdAsync(bookId);
            var viewModels = new List<ReviewViewModel>();

            foreach (var review in reviews)
            {
                viewModels.Add(new ReviewViewModel
                {
                    ReviewId = review.ReviewId,
                    BookId = review.BookId,
                    UserId = review.UserId,
                    UserName = review.User?.FullName ?? "Anonymous",
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt
                });
            }

            return viewModels;
        }

        public async Task<BookRatingViewModel> GetBookRatingAsync(int bookId)
        {
            var reviews = await _reviewRepository.GetByBookIdAsync(bookId);
            var reviewList = reviews.ToList();

            return new BookRatingViewModel
            {
                AverageRating = reviewList.Any() ? reviewList.Average(r => r.Rating) : 0,
                TotalReviews = reviewList.Count,
                FiveStarCount = reviewList.Count(r => r.Rating == 5),
                FourStarCount = reviewList.Count(r => r.Rating == 4),
                ThreeStarCount = reviewList.Count(r => r.Rating == 3),
                TwoStarCount = reviewList.Count(r => r.Rating == 2),
                OneStarCount = reviewList.Count(r => r.Rating == 1)
            };
        }

        public async Task<List<ReviewViewModel>> GetUserReviewsAsync(string userId)
        {
            var reviews = await _reviewRepository.GetByUserIdAsync(userId);
            return reviews.Select(r => new ReviewViewModel
            {
                ReviewId = r.ReviewId,
                BookId = r.BookId,
                UserId = r.UserId,
                UserName = r.User?.FullName ?? "Anonymous",
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        public async Task<bool> HasUserReviewedBookAsync(string userId, int bookId)
        {
            return await _reviewRepository.HasUserReviewedBookAsync(userId, bookId);
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, string userId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null || review.UserId != userId)
                return false;

            await _reviewRepository.DeleteAsync(reviewId);
            await _reviewRepository.SaveChangesAsync();
            return true;
        }
    }
}
