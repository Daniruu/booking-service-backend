using BookingService.Extensions;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    /// <summary>
    /// Handles operations related to user's favorite businesses.
    /// </summary>
    [ApiController]
    [Route("api/favorites")]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoritesService _favoritesService;
        private readonly ILogger<FavoritesController> _logger;

        public FavoritesController(IFavoritesService favoritesService, ILogger<FavoritesController> logger)
        {
            _favoritesService = favoritesService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the list of businesses favorited by the current user.
        /// </summary>
        /// <returns>
        /// 200 OK with list of favorites; 401 Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetFavoritesBusinesses()
        {
            try
            {
                var userId = User.GetUserId();

                _logger.LogInformation("Retrieving favorite businesses for user {UserId}", userId);
                var result = await _favoritesService.GetFavoritesAsync(userId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to retrieve favorites for user {UserId}: {Reason}", userId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return Ok(result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during retrieval of user favorites.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Checks if the specified business is in the current user's favorites.
        /// </summary>
        /// <param name="businessId">ID of the business to check.</param>
        /// <returns>
        /// 200 OK with boolean result; 401 Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpGet("{businessId}")]
        public async Task<IActionResult> IsBusinessFavorite(int businessId)
        {
            try
            {
                var userId = User.GetUserId();

                _logger.LogInformation("Checking if business {BusinessId} is favorite for user {UserId}", businessId, userId);
                var result = await _favoritesService.IsFavoriteAsync(userId, businessId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to check favorite for user {UserId}, business {BusinessId}: {Reason}", userId, businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return Ok(new { isFavorite = result.Data });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Adds the specified business to the current user's favorites.
        /// </summary>
        /// <param name="businessId">ID of the business to add.</param>
        /// <returns>
        /// 204 No Content if successful; 
        /// 409 Conflict if already in favorites;
        /// 404 if business not found; 
        /// 401 Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpPost("{businessId}")]
        public async Task<IActionResult> AddToFavorite(int businessId)
        {
            try
            {
                var userId = User.GetUserId();

                _logger.LogInformation("Adding business {BusinessId} to favorites for user {UserId}", businessId, userId);
                var result = await _favoritesService.AddFavoriteAsync(userId, businessId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to add favorite for user {UserId}, business {BusinessId}: {Reason}", userId, businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Removes the specified business from the current user's favorites.
        /// </summary>
        /// <param name="businessId">ID of the business to remove.</param>
        /// <returns>
        /// 204 No Content if successful;
        /// 404 if not in favorites;
        /// 401 Unauthorized if not authenticated.
        /// </returns>
        [HttpDelete("{businessId}")]
        public async Task<IActionResult> RemoveFromFavorite(int businessId)
        {
            try
            {
                var userId = User.GetUserId();

                _logger.LogInformation("Removing business {BusinessId} from favorites for user {UserId}", businessId, userId);

                var result = await _favoritesService.RemoveFavoriteAsync(userId, businessId);
                if (!result.Success)
                {
                    _logger.LogWarning("Failed to remove favorite for user {UserId}, business {BusinessId}: {Reason}", userId, businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt.");
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
