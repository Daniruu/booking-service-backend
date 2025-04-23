using BookingService.Data;
using BookingService.Models;
using BookingService.Specifications;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BookingServiceDbContext _context;
        public UserRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(int id, UserSpecifications? spec = null)
        {
            IQueryable<User> query = _context.Users;

            if (spec?.IncludeBookings == true)
                query = query.Include(u => u.Bookings);

            if (spec?.IncludeReviews == true)
                query = query.Include(u => u.Reviews);

            if (spec?.IncludeFavorites == true)
                query = query.Include(u => u.FavoriteBusinesses);

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}
