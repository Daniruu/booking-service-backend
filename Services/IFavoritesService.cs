using BookingService.DTOs;
using BookingService.Utils;

namespace BookingService.Services
{
    /// <summary>
    /// Service for managing user favorite businesses.
    /// </summary>
    public interface IFavoritesService
    {
        /// <summary>
        /// Gets the list of businesses favorited by the user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>List of favorited businesses.</returns>
        Task<ServiceResult<IEnumerable<BusinessListItemDto>>> GetFavoritesAsync(int userId);

        /// <summary>
        /// Checks whether a specific business is in the user's favorites.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="businessId">The ID of the business.</param>
        /// <returns>True if business is favorited; otherwise, false.</returns>
        Task<ServiceResult<bool>> IsFavoriteAsync(int userId, int businessId);

        /// <summary>
        /// Adds a business to the user's favorites.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="businessId">The ID of the business.</param>
        /// <returns>Service result indicating success or failure.</returns>
        Task<ServiceResult> AddFavoriteAsync(int userId, int businessId);

        // <summary>
        /// Removes a business from the user's favorites.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="businessId">The ID of the business.</param>
        /// <returns>Service result indicating success or failure.</returns>
        Task<ServiceResult> RemoveFavoriteAsync(int userId, int businessId);
    }
}
