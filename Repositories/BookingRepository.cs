using BookingService.Data;
using BookingService.Models;
using BookingService.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookingService.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingServiceDbContext _context;

        public BookingRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Business)
                .Include(b => b.Employee)
                .Include(b => b.Service)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Booking>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Business)
                    .ThenInclude(b => b.Images)
                .Include(b => b.Employee)
                .Include(b => b.Service)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetAllByBusinessIdAsync(int businessId)
        {
            return await _context.Bookings
                .Where(b => b.BusinessId == businessId)
                .Include(b => b.User)
                .Include(b => b.Employee)
                .Include(b => b.Service)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetAllByEmployeeIdAsync(int employeeId, DateTimeOffset? date)
        {
            var query = _context.Bookings.Where(b => b.EmployeeId == employeeId);

            if (date.HasValue)
            {
                var startOfDayUtc = date.Value.UtcDateTime.Date;
                var endOfDayUtc = startOfDayUtc.AddDays(1);

                query = query.Where(b => b.StartTime >= startOfDayUtc && b.StartTime < endOfDayUtc);
            }
                

            return await query.ToListAsync();
        }

        public async Task AddRangeAsync(List<Booking> bookings)
        {
            if (bookings == null || !bookings.Any())
                throw new ArgumentException("Booking list cannot be null or empty.", nameof(bookings));

            await _context.Bookings.AddRangeAsync(bookings);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<Booking, bool>> predicate)
        {
            return await _context.Bookings.AnyAsync(predicate);
        }
    }
}
