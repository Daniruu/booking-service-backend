using BookingService.Data;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories
{
    public class FavoritesRepository : IFavoritesRepository
    {
        private readonly BookingServiceDbContext _context;

        public FavoritesRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FavoriteBusiness>> GetByUserIdAsync(int userId)
        {
            return await _context.FavoriteBusinesses
                .Where(fb => fb.UserId == userId)
                .Include(fb => fb.Business)
                    .ThenInclude(b => b.Images)
                .ToListAsync();
        }

        public async Task<bool> FavoriteExists(int userId, int businesId)
        {
            return await _context.FavoriteBusinesses
                .AnyAsync(fb => fb.UserId == userId && fb.BusinessId == businesId);
        }

        public async Task AddToFavoritesAsync(FavoriteBusiness favoriteBusiness)
        {
            await _context.FavoriteBusinesses.AddAsync(favoriteBusiness);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromFavoritesAsync(int userId, int businessId)
        {
            var favorite = await _context.FavoriteBusinesses.FirstOrDefaultAsync(fb => fb.UserId == userId && fb.BusinessId == businessId);

            if (favorite != null)
            {
                _context.FavoriteBusinesses.Remove(favorite);
                await _context.SaveChangesAsync();
            }
        }
    }
}
