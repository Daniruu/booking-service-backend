using BookingService.DTOs;
using BookingService.Utils;

namespace BookingService.Services
{
    public interface IReviewService
    {
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
        Task<ServiceResult<ReviewDto>> CreateReviewAsync(int businessId, int userId, CreateReviewDto reviewDto);
    }
}
