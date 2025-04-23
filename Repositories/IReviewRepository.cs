using BookingService.Models;

namespace BookingService.Repositories
{
    public interface IReviewRepository
    {
        Task AddAsync(Review review);
        Task<Review?> GetByUserAndBusinessAsync(int userId, int businessId);
        Task<bool> ExistsByUserAndBusinessAsync(int userId, int businessId);
        Task AddImagesAsync(IEnumerable<ReviewImage> images);
    }
}
