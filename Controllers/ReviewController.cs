using BookingService.DTOs;
using BookingService.Extensions;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    /// <summary>
    /// Controller responsible for managing reviews created by authenticated users.
    /// </summary>
    [ApiController]
    [Route("api/reviews")]

    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(IReviewService reviewService, ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new review for a specific business by the authenticated user.
        /// </summary>
        /// <param name="businessId">The ID of the business being reviewed.</param>
        /// <param name="reviewDto">The review creation data including rating, comment, and optional images.</param>
        /// <returns>
        /// 200 OK with the created review;
        /// 400 Bad Request if input is invalid;
        /// 401 Unauthorized if user is not authenticated;
        /// 404 Not Found if the business does not exist or user is not allowed to review.
        /// </returns>
        [HttpPost("{businessId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateReview(int businessId, [FromForm] ReviewCreateDto reviewDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid review model for business {BusinessId}.", businessId);
                return BadRequest(ModelState);
            }

            try
            {
                var userId = User.GetUserId();

                _logger.LogInformation("User {UserId} is attempting to create review for business {BusinessId}.", userId, businessId);
                var result = await _reviewService.CreateReviewAsync(businessId, userId, reviewDto);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to create review for business {BusinessId} by user {UserId}: {Reason}", businessId, userId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Review successfully created for business {BusinessId} by user {UserId}.", businessId, userId);
                return Ok(result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt while creating review.");
                return Unauthorized(new { message = ex.Message });
            }
        }

    }
}
