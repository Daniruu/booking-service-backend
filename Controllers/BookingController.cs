using BookingService.DTOs;
using BookingService.Extensions;
using BookingService.Models;
using BookingService.Services;
using BookingService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    /// <summary>
    /// Handles creation, retrieval and state management of bookings for both users and businesses.
    /// </summary>
    /// <remarks>
    /// All endpoints require authentication unless explicitly marked as public.
    /// Business and user access is separated to ensure proper data isolation.
    /// </remarks>
    [ApiController]
    [Route("api/bookings")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingController> _logger;
        public BookingController(IBookingService bookingService, ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all bookings made by the currently authenticated user.
        /// </summary>
        /// <returns>
        /// 200 OK with a list of user's bookings;
        /// 401 Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpGet("user")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserBookings()
        {
            try
            {
                var userId = User.GetUserId();

                _logger.LogInformation("Retrieving bookings for user {UserId}", userId);
                var result = await _bookingService.GetByUserIdAsync(userId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to retrieve booking for user {UserId}: {Reason}", userId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return Ok(result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt to user bookings.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all bookings related to the current authenticated business account.
        /// </summary>
        /// <returns>
        /// 200 OK with a list of business bookings;
        /// 401 Unauthorized if the user is not authenticated;
        /// 403 Forbidden if the user does not own or manage a business.
        /// </returns>
        [HttpGet("business")]
        [Authorize(Roles = "Business")]
        public async Task<IActionResult> GetBusinessBookings()
        {
            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Retrieving bookings for business {BusinessId}", businessId);
                var result = await _bookingService.GetByBusinessIdAsync(businessId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to retrieve booking for business {BusinessId}: {Reason}", businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return Ok(result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt to business bookings.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Returns available time slots for a specific service on a given date.
        /// </summary>
        /// <param name="serviceId">The ID of the service.</param>
        /// <param name="selectedDate">The date to check availability for (format: yyyy-MM-dd).</param>
        /// <returns>
        /// 200 OK with a list of available time slots;
        /// 400 Bad Request if input is invalid;
        /// 401 Unauthorized if not authenticated.
        /// </returns>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableTimeSlots([FromQuery] int serviceId, [FromQuery] DateTimeOffset selectedDate)
        {
            if (serviceId <= 0)
            {
                _logger.LogWarning("Invalid service ID in GetAvailableTimeSlots.");
                return BadRequest(new { message = "Invalid service ID." });
            }

            try
            {
                _logger.LogInformation("Retrieving available time slots for service {ServiceId} on {Date}.", serviceId, selectedDate);

                var result = await _bookingService.GetAvailableTimeSlotsAsync(serviceId, selectedDate);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to get time slots for service {ServiceId}: {Reason}", serviceId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return Ok(result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt to available time slots.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Creates one or more bookings for the currently authenticated user.
        /// </summary>
        /// <param name="dtos">A list of booking creation requests.</param>
        /// <returns>
        /// 201 Created with the created bookings;
        /// 400 Bad Request if the request is invalid;
        /// 401 Unauthorized if the user is not authenticated.
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateBookings([FromBody] List<BookingCreateDto> dtos)
        {
            if (!ModelState.IsValid || dtos == null || !dtos.Any())
            {
                _logger.LogWarning("Invalid model state in CreateBookings. ModelState: {State}", ModelState);
                return BadRequest(new { message = "Invalid booking data." });
            }

            try
            {
                var userId = User.GetUserId();

                _logger.LogInformation("Creating {Count} booking(s) for user {UserId}.", dtos.Count, userId);
                var result = await _bookingService.CreateBookingsAsync(userId, dtos);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to create bookings for user {UserId}: {Reason}", userId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Successfully created {Count} booking(s) for user {UserId}.", result.Data.Count, userId);
                return StatusCode(StatusCodes.Status201Created, result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during booking creation.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the status of a booking (confirm or reject) by the business.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to update.</param>
        /// <param name="dto">The status update request (allowed values: confirmed, rejected).</param>
        /// <returns>
        /// 200 OK if the update was successful;
        /// 400 Bad Request for invalid input;
        /// 401 Unauthorized if not authenticated;
        /// 403 Forbidden if the booking does not belong to the business;
        /// 404 Not Found if the booking does not exist.
        /// </returns>
        [HttpPatch("{bookingId}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int bookingId, [FromBody] BookingStatusUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid status update for booking {BookingId}.", bookingId);
                return BadRequest(ModelState);
            }

            try
            {
                var accountId = User.GetUserId();
                var role = User.GetUserRole();

                _logger.LogInformation("Status update request from {Role}. BookingId: {BookingId}, NewStatus: {Status}", role, bookingId, dto.Status);

                if (role == "Business")
                {
                    _logger.LogInformation("Updating status for booking {BookingId} to '{Status}' by business {BusinessId}.", bookingId, dto.Status, accountId);
                    var result = await _bookingService.UpdateBookingStatusByBusinessAsync(bookingId, accountId, dto.Status);

                    if (!result.Success)
                    {
                        _logger.LogWarning("Failed to update status for booking {BookingId}: {Reason}", bookingId, result.ErrorMessage);
                        return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                    }

                    _logger.LogInformation("Booking {BookingId} status updated to '{Status}'.", bookingId, dto.Status);
                    return Ok(result.Data);
                } 
                else if (role == "User")
                {
                    _logger.LogInformation("Updating status for booking {BookingId} to '{Status}' by user {UserId}.", bookingId, dto.Status, accountId);
                    var result = await _bookingService.UpdateBookingStatusByUserAsync(bookingId, accountId, dto.Status);

                    if (!result.Success)
                    {
                        _logger.LogWarning("Failed to update status for booking {BookingId}: {Reason}", bookingId, result.ErrorMessage);
                        return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                    }

                    _logger.LogInformation("Booking {BookingId} status updated to '{Status}'.", bookingId, dto.Status);
                    return Ok(result.Data);
                }
                else
                {
                    return Forbid();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt while updating booking {BookingId}.", bookingId);
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
