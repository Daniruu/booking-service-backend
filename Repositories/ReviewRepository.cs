using BookingService.Data;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly BookingServiceDbContext _context;

        public ReviewRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task<Review?> GetByUserAndBusinessAsync(int userId, int businessId)
        {
            return await _context.Reviews
                .Where(r => r.UserId == userId && r.BusinessId == businessId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsByUserAndBusinessAsync(int userId, int businessId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.UserId == userId && r.BusinessId == businessId);
        }

        public async Task AddImagesAsync(IEnumerable<ReviewImage> images)
        {
            await _context.ReviewImages.AddRangeAsync(images);
            await _context.SaveChangesAsync();
        }
    }
}
