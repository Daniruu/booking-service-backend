using BookingService.Data;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories
{
    public class BusinessImageRepository : IBusinessImageRepository
    {
        private readonly BookingServiceDbContext _context;

        public BusinessImageRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task<List<BusinessImage>> GetByBusinessIdAsync(int businessId)
        {
            return await _context.BusinessImages.Where(img => img.BusinessId == businessId).ToListAsync();
        }

        public async Task<BusinessImage?> GetByIdAsync(int id)
        {
            return await _context.BusinessImages.FindAsync(id);
        }

        public async Task AddAsync(BusinessImage image)
        {
            await _context.BusinessImages.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BusinessImage image)
        {
            _context.BusinessImages.Update(image);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(BusinessImage image)
        {
            _context.BusinessImages.Remove(image);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAllByBusinessIdAsync(int businessId)
        {
            var images = await _context.BusinessImages.Where(img => img.BusinessId == businessId).ToListAsync();

            _context.BusinessImages.RemoveRange(images);
            await _context.SaveChangesAsync();
        }
    }
}
