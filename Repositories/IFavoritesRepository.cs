using BookingService.Models;

namespace BookingService.Repositories
{
    public interface IFavoritesRepository
    {
        Task<IEnumerable<FavoriteBusiness>> GetByUserIdAsync(int userId);
        Task<bool> FavoriteExists(int userId, int businesId);
        Task AddToFavoritesAsync(FavoriteBusiness favoriteBusiness);
        Task RemoveFromFavoritesAsync(int userId, int businessId);
    }
}
