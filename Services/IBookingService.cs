using BookingService.DTOs;
using BookingService.Models;
using BookingService.Utils;

namespace BookingService.Services
{
    /// <summary>
    /// Service responsible for managing booking operations including creation, retrieval,
    /// status updates, and availability checks. Handles both business and user perspectives.
    /// </summary>
    /// <remarks>
    /// This service ensures proper validation, authorization, time conflict prevention,
    /// and supports logic for auto-confirmation and buffer time restrictions.
    /// It also triggers appropriate notifications when booking states change.
    /// </remarks>
    public interface IBookingService
    {
        /// <summary>
        /// Retrieves all bookings made by a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose bookings to retrieve.</param>
        /// <returns>
        /// A <see cref="ServiceResult{T}"/> containing a list of <see cref="UserBookingDto"/>s;
        /// or an error if the ID is invalid or an exception occurs.
        /// </returns>
        Task<ServiceResult<IEnumerable<UserBookingDto>>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Retrieves all bookings associated with a specific business.
        /// </summary>
        /// <param name="businessId">The ID of the business whose bookings should be retrieved.</param>
        /// <returns>
        /// A <see cref="ServiceResult{T}"/> containing a list of <see cref="BusinessBookingDto"/>s;
        /// or an error if the business ID is invalid or an exception occurs.
        /// </returns>
        Task<ServiceResult<IEnumerable<BusinessBookingDto>>> GetByBusinessIdAsync(int businessId);

        /// <summary>
        /// Returns a list of available time slots for the specified service and date.
        /// Takes into account the schedules of the business and employee,
        /// existing bookings, duration of the service, and buffer times.
        /// </summary>
        /// <param name="serviceId">The ID of the service.</param>
        /// <param name="selectedDate">The date for which availability is being checked (UTC).</param>
        /// <returns>
        /// A <see cref="ServiceResult{T}"/> containing a list of available time slots;
        /// or an error with appropriate status code.
        /// </returns>
        Task<ServiceResult<List<DateTimeOffset>>> GetAvailableTimeSlotsAsync(int serviceId, DateTimeOffset selectedDate);

        /// <summary>
        /// Creates multiple bookings for the specified user based on selected services and time slots.
        /// Validates availability and prevents time conflicts using business rules.
        /// </summary>
        /// <param name="userId">The ID of the user making the bookings.</param>
        /// <param name="dtos">A list of booking creation requests.</param>
        /// <returns>
        /// A <see cref="ServiceResult{List{UserBookingDto}}"/> with successfully created bookings;
        /// or an error if the input is invalid or time slots are unavailable.
        /// </returns>
        Task<ServiceResult<List<UserBookingDto>>> CreateBookingsAsync(int userId, List<CreateBookingDto> dtos);

        /// <summary>
        /// Updates the status of a booking (e.g., confirm or reject) if it belongs to the given business.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to update.</param>
        /// <param name="businessId">The ID of the authenticated business.</param>
        /// <param name="newStatus">The new status to apply (Active or Canceled).</param>
        /// <returns>
        /// A <see cref="ServiceResult{BusinessBookingDto}"/> with the updated booking;
        /// or an error if not found or not authorized.
        /// </returns>
        Task<ServiceResult<BusinessBookingDto>> UpdateBookingStatusByBusinessAsync(int bookingId, int businessId, BookingStatus newStatus);

        /// <summary>
        /// Allows a user to cancel their own booking.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to update.</param>
        /// <param name="userId">The ID of the authenticated business.</param>
        /// <param name="newStatus">The new status to apply (Active or Canceled).</param>
        /// <returns>
        /// A <see cref="ServiceResult{UserBookingDto}"/> with the updated booking;
        /// or an error if not found or not authorized.
        /// </returns>
        Task<ServiceResult<UserBookingDto>> UpdateBookingStatusByUserAsync(int bookingId, int userId, BookingStatus newStatus);

        Task<bool> UserHasCompleteBooking(int userId, int businessId);
    }
}
