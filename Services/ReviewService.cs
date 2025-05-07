using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Utils;

namespace BookingService.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;
        private readonly IGoogleCloudStorageService _googleCloudStorageService;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(
            IReviewRepository reviewRepository, 
            IMapper mapper, 
            IBookingService bookingService, 
            IGoogleCloudStorageService googleCloudStorageService,
            ILogger<ReviewService> logger)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _bookingService = bookingService;
            _googleCloudStorageService = googleCloudStorageService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new review for a specific business by the specified user.
        /// Validates ownership, existing review, and ability to review.
        /// </summary>
        /// <param name="businessId">The ID of the business being reviewed.</param>
        /// <param name="userId">The ID of the user writing the review.</param>
        /// <param name="dto">The review data including rating, comment, and images.</param>
        /// <returns>
        /// A <see cref="ServiceResult{ReviewDto}"/> containing the newly created review;
        /// or an error with appropriate status code and message.
        /// </returns>
        public async Task<ServiceResult<ReviewDto>> CreateReviewAsync(int businessId, int userId, CreateReviewDto dto)
        {
            if (businessId < 0 || userId < 0)
            {
                _logger.LogWarning("Invalid user or business ID. UserId: {UserId}, BusinessId: {BusinessId}", userId, businessId);
                return ServiceResult<ReviewDto>.Failure("Invalid user or business ID.", 400);
            }

            // Check if user has already reviewed this business
            if (await _reviewRepository.ExistsByUserAndBusinessAsync(userId, businessId))
            {
                _logger.LogWarning("User {UserId} has already reviewed business {BusinessId}.", userId, businessId);
                return ServiceResult<ReviewDto>.Failure("You have already reviewed this business.", 400);
            }

            // Check if user is allowed to review (e.g., completed booking exists)
            bool canReview = await _bookingService.UserHasCompleteBooking(userId, businessId);
            if (!canReview)
            {
                _logger.LogWarning("User {UserId} attempted to review business {BusinessId} without eligible booking.", userId, businessId);
                return ServiceResult<ReviewDto>.Failure("You can only review businesses after completing a booking.", 403);
            }

            var review = _mapper.Map<Review>(dto);
            review.UserId = userId;
            review.BusinessId = businessId;

            await _reviewRepository.AddAsync(review);

            // Handle optional image upload
            if (dto.Images != null && dto.Images.Any())
            {
                var reviewImages = new List<ReviewImage>();
                foreach (var file in dto.Images)
                {
                    var imageUrl = await _googleCloudStorageService.UploadFileAsync("reviews", file);
                    reviewImages.Add(new ReviewImage { ReviewId = review.Id, Url = imageUrl });
                }

                await _reviewRepository.AddImagesAsync(reviewImages);
                review.Images = reviewImages;
            }

            var dtoResult = _mapper.Map<ReviewDto>(review);
            _logger.LogInformation("Review {ReviewId} created by user {UserId} for business {BusinessId}.", review.Id, userId, businessId);
            return ServiceResult<ReviewDto>.SuccessResult(dtoResult);
        }
    }
}
