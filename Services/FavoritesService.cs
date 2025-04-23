using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Utils;

namespace BookingService.Services
{
    /// <summary>
    /// Service for managing user favorite businesses.
    /// </summary>
    public class FavoritesService : IFavoritesService
    {
        private readonly IFavoritesRepository _favoritesRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<FavoritesService> _logger;

        public FavoritesService(IFavoritesRepository favoritesRepository,  IMapper mapper, ILogger<FavoritesService> logger, IBusinessRepository businessRepository)
        {
            _favoritesRepository = favoritesRepository;
            _mapper = mapper;
            _logger = logger;
            _businessRepository = businessRepository;
        }

        /// <summary>
        /// Gets the list of businesses favorited by the user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>List of favorited businesses.</returns>
        public async Task<ServiceResult<IEnumerable<BusinessListItemDto>>> GetFavoritesAsync(int userId)
        {
            var favorites = await _favoritesRepository.GetByUserIdAsync(userId);
            var businesses = favorites.Select(fb => fb.Business).ToList();
            var businessDtos = _mapper.Map<IEnumerable<BusinessListItemDto>>(businesses);

            _logger.LogInformation("Retrieved {Count} favorite businesses for user {UserId}", businesses.Count, userId);
            return ServiceResult<IEnumerable<BusinessListItemDto>>.SuccessResult(businessDtos);
        }

        /// <summary>
        /// Checks whether a specific business is in the user's favorites.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="businessId">The ID of the business.</param>
        /// <returns>True if business is favorited; otherwise, false.</returns>
        public async Task<ServiceResult<bool>> IsFavoriteAsync(int userId, int businessId)
        {
            var exists = await _favoritesRepository.FavoriteExists(userId, businessId);

            _logger.LogInformation("Checked favorite status for user {UserId} and business {BusinessId}: {Result}", userId, businessId, exists);
            return ServiceResult<bool>.SuccessResult(exists);
            return ServiceResult<bool>.SuccessResult(exists);
        }

        /// <summary>
        /// Adds a business to the user's favorites.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="businessId">The ID of the business.</param>
        /// <returns>Service result indicating success or failure.</returns>
        public async Task<ServiceResult> AddFavoriteAsync(int userId, int businessId)
        {
            var exists = await _favoritesRepository.FavoriteExists(userId, businessId);
            if (exists)
            {
                _logger.LogWarning("User {UserId} tried to favorite Business {BusinessId}, but it's already added", userId, businessId);
                return ServiceResult.Failure("Business already in favorites.", 409);
            }

            var business = await _businessRepository.GetByIdAsync(businessId);
            if (business == null || !business.IsPublished)
            {
                _logger.LogWarning("Business {BusinessId} not found or unpublished", businessId);
                return ServiceResult.Failure("Business not found or not published.", 404);
            }

            var favoriteBusiness = new FavoriteBusiness { UserId = userId, BusinessId = businessId };
            await _favoritesRepository.AddToFavoritesAsync(favoriteBusiness);

            _logger.LogInformation("Business {BusinessId} added to favorites for user {UserId}", businessId, userId);
            return ServiceResult.SuccessResult();
        }

        // <summary>
        /// Removes a business from the user's favorites.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="businessId">The ID of the business.</param>
        /// <returns>Service result indicating success or failure.</returns>
        public async Task<ServiceResult> RemoveFavoriteAsync(int userId, int businessId)
        {
            await _favoritesRepository.RemoveFromFavoritesAsync(userId, businessId);
            _logger.LogInformation("Business {BusinessId} removed from favorites for user {UserId}", businessId, userId);
            return ServiceResult.SuccessResult();
        }
    }
}
