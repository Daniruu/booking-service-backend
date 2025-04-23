using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Specifications;
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
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly ILogger<BookingService> _logger;
        public BookingService(
            IBookingRepository bookingRepository,
            IServiceRepository serviceRepository,
            IMapper mapper,
            INotificationService notificationService,
            ILogger<BookingService> logger
        )
        {
            _bookingRepository = bookingRepository;
            _serviceRepository = serviceRepository;
            _mapper = mapper;
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all bookings made by a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose bookings to retrieve.</param>
        /// <returns>
        /// A <see cref="ServiceResult{T}"/> containing a list of <see cref="UserBookingDto"/>s;
        /// or an error if the ID is invalid or an exception occurs.
        /// </returns>
        public async Task<ServiceResult<IEnumerable<UserBookingDto>>> GetByUserIdAsync(int userId)
        {
            if (userId < 0)
            {
                _logger.LogWarning("Invalid user ID received in GetByUserIdAsync: {UserId}", userId);
                return ServiceResult<IEnumerable<UserBookingDto>>.Failure("Invalid user ID.", 400);
            }

            var bookings = await _bookingRepository.GetAllByUserIdAsync(userId);
            var bookingDtos = _mapper.Map<List<UserBookingDto>>(bookings);

            _logger.LogInformation("Successfully retrieved {Count} bookings for user {UserId}.", bookingDtos.Count, userId);
            return ServiceResult<IEnumerable<UserBookingDto>>.SuccessResult(bookingDtos);
        }

        /// <summary>
        /// Retrieves all bookings associated with a specific business.
        /// </summary>
        /// <param name="businessId">The ID of the business whose bookings should be retrieved.</param>
        /// <returns>
        /// A <see cref="ServiceResult{T}"/> containing a list of <see cref="BusinessBookingDto"/>s;
        /// or an error if the business ID is invalid or an exception occurs.
        /// </returns>
        public async Task<ServiceResult<IEnumerable<BusinessBookingDto>>> GetByBusinessIdAsync(int businessId)
        {
            if (businessId < 0)
            {
                _logger.LogWarning("Invalid business ID received in GetByBusinessIdAsync: {BusinessId}", businessId);
                return ServiceResult<IEnumerable<BusinessBookingDto>>.Failure("Invalid business ID.", 400);
            }

            var bookings = await _bookingRepository.GetAllByBusinessIdAsync(businessId);
            var bookingDtos = _mapper.Map<List<BusinessBookingDto>>(bookings);

            _logger.LogInformation("Successfully retrieved {Count} bookings for business {BusinessId}.", bookingDtos.Count, businessId);
            return ServiceResult<IEnumerable<BusinessBookingDto>>.SuccessResult(bookingDtos);
        }

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
        public async Task<ServiceResult<List<DateTimeOffset>>> GetAvailableTimeSlotsAsync(int serviceId, DateTimeOffset selectedDate)
        {
            var now = DateTimeOffset.UtcNow;

            if (selectedDate.Date < now.Date)
            {
                _logger.LogWarning("Attempt to get slots for past date: {Date}", selectedDate);
                return ServiceResult<List<DateTimeOffset>>.Failure("Cannot check past dates.", 400);
            }

            _logger.LogInformation("Checking availability for service {ServiceId} on {Date}.", serviceId, selectedDate.Date);

            var spec = new ServiceSpecifications
            {
                IncludeBusiness = true,
                IncludeEmployee = true,
            };

            var service = await _serviceRepository.GetByIdAsync(serviceId, spec);
            if (service == null)
            {
                _logger.LogWarning("Service {ServiceId} not found.", serviceId);
                return ServiceResult<List<DateTimeOffset>>.Failure("Service not found.", 404);
            }

            var business = service.Business;
            var employee = service.Employee;

            if (business == null || employee == null)
            {
                _logger.LogWarning("Service {ServiceId} is missing associated business or employee.", serviceId);
                return ServiceResult<List<DateTimeOffset>>.Failure("Invalid business or employee.", 400);
            }

            var businessSchedule = business.Schedule.FirstOrDefault(s => s.Day == selectedDate.DayOfWeek);
            var employeeSchedule = employee.Schedule.FirstOrDefault(s => s.Day == selectedDate.DayOfWeek);

            if (businessSchedule == null || employeeSchedule == null)
            {
                _logger.LogWarning("Missing schedule for business or employee on {Day}.", selectedDate.DayOfWeek);
                return ServiceResult<List<DateTimeOffset>>.Failure("Invalid business or employee schedule.", 400);
            }

            var bufferTime = business.Settings.BookingBufferTime;

            var exitingBookings = (await _bookingRepository.GetAllByEmployeeIdAsync(employee.Id, selectedDate))
                .Where(b => b.Status == BookingStatus.Active || b.Status == BookingStatus.Pending)
                .Select(b => new { Start = b.StartTime, End = b.EndTime })
                .ToList();

            var availableSlots = new List<DateTimeOffset>();

            foreach (var timeSlot in businessSchedule.TimeSlots)
            {
                var startTime = selectedDate.Date + timeSlot.StartTime;
                var endTime = startTime + service.Duration;

                var minStartTime = now.UtcDateTime.AddMinutes(15);
                if (selectedDate.Date == now.Date && startTime < minStartTime)
                    startTime = minStartTime;

                while (startTime.TimeOfDay + service.Duration <= timeSlot.EndTime)
                {
                    bool isAvailable = !exitingBookings.Any(b =>
                        (startTime < b.End + bufferTime && endTime > b.Start - bufferTime));

                    if (isAvailable)
                        availableSlots.Add(startTime);

                    startTime = startTime.Add(TimeSpan.FromMinutes(15));
                    endTime = startTime + service.Duration;
                }
            }

            _logger.LogInformation("Found {Count} available time slots for service {ServiceId}.", availableSlots.Count, serviceId);
            return ServiceResult<List<DateTimeOffset>>.SuccessResult(availableSlots);
        }

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
        public async Task<ServiceResult<List<UserBookingDto>>> CreateBookingsAsync(int userId, List<BookingCreateDto> dtos)
        {
            if (userId < 0)
            {
                _logger.LogWarning("Invalid user ID: {UserId}", userId);
                return ServiceResult<List<UserBookingDto>>.Failure("Invalid user ID.", 400);
            }

            if (dtos == null || !dtos.Any())
            {
                _logger.LogWarning("Empty booking list received.");
                return ServiceResult<List<UserBookingDto>>.Failure("No services selected.", 400);
            }

            var now = DateTimeOffset.UtcNow;
            var serviceIds = dtos.Select(s => s.ServiceId).Distinct().ToList();

            _logger.LogInformation("Attempting to create {Count} bookings for user {UserId}.", dtos.Count, userId);

            var spec = new ServiceSpecifications
            {
                IncludeBusiness = true,
                IncludeEmployee = true,
            };

            var services = await _serviceRepository.GetByIdsAsync(serviceIds, spec);
            if (services.Count != serviceIds.Count)
            {
                _logger.LogWarning("Some services not found: requested {RequestedCount}, found {FoundCount}.", serviceIds.Count, services.Count);
                return ServiceResult<List<UserBookingDto>>.Failure("Some services not found.", 404);
            }

            var bookings = new List<Booking>();

            foreach (var dto in dtos)
            {
                var service = services.FirstOrDefault(s => s.Id == dto.ServiceId);
                if (service == null)
                    continue;

                var business = service.Business;
                var employee = service.Employee;

                if (business == null || employee == null)
                {
                    _logger.LogWarning("Service {ServiceId} is missing business or employee.", service.Id);
                    return ServiceResult<List<UserBookingDto>>.Failure($"Invalid business or employee for service {service.Id}.", 400);
                }

                var bufferTime = business.Settings.BookingBufferTime;
                var startTime = dto.StartTime.ToUniversalTime();
                var endTime = startTime + service.Duration;

                var existingBookings = (await _bookingRepository.GetAllByEmployeeIdAsync(employee.Id, startTime))
                    .Where(b => b.Status == BookingStatus.Active || b.Status == BookingStatus.Pending)
                    .ToList();

                bool isAvailable = !existingBookings.Any(b =>
                    (startTime < b.EndTime + bufferTime && endTime > b.StartTime - bufferTime))
                    && startTime > now.AddMinutes(15);

                if (!isAvailable)
                {
                    _logger.LogWarning("Time slot not available for service {ServiceId} at {StartTime}", service.Id, startTime);
                    return ServiceResult<List<UserBookingDto>>.Failure($"Selected time slot for service {service.Id} is not available", 400);
                }

                var booking = new Booking
                {
                    UserId = userId,
                    EmployeeId = employee.Id,
                    BusinessId = business.Id,
                    ServiceId = service.Id,
                    StartTime = startTime,
                    EndTime = endTime,
                    FinalPrice = service.Price,
                    Note = dto.Note,
                    CreatedAt = now.ToUniversalTime(),
                    Status = business.Settings.AutoConfirmBookings ? BookingStatus.Active : BookingStatus.Pending
                };

                bookings.Add(booking);
            }

            if (!bookings.Any())
            {
                _logger.LogWarning("No valid bookings created for user {UserId}.", userId);
                return ServiceResult<List<UserBookingDto>>.Failure("No valid bookings could be created.", 400);
            }

            await _bookingRepository.AddRangeAsync(bookings);

            foreach (var booking in bookings.Where(b => b.Status == BookingStatus.Pending))
            {
                await _notificationService.SendBookingRequestNotification(booking);
            }

            _logger.LogInformation("Successfully created {Count} bookings for user {UserId}.", bookings.Count, userId);
            var bookingDtos = _mapper.Map<List<UserBookingDto>>(bookings);

            return ServiceResult<List<UserBookingDto>>.SuccessResult(bookingDtos);
        }

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
        public async Task<ServiceResult<BusinessBookingDto>> UpdateBookingStatusByBusinessAsync(int bookingId, int businessId, BookingStatus newStatus)
        {
            if (bookingId < 0 || businessId < 0)
            {
                _logger.LogWarning("Invalid IDs in UpdateBookingStatusByBusinessAsync. BookingId: {BookingId}, BusinessId: {BusinessId}", bookingId, businessId);
                return ServiceResult<BusinessBookingDto>.Failure("Invalid identifiers.", 400);
            }

            if (!Enum.IsDefined(typeof(BookingStatus), newStatus) || !(newStatus == BookingStatus.Active || newStatus == BookingStatus.Canceled))
            {
                _logger.LogWarning("Invalid booking status value: {Status}", newStatus.ToString());
                return ServiceResult<BusinessBookingDto>.Failure("Invalid status value.", 400);
            }

            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
            {
                _logger.LogWarning("Booking {BookingId} not found.", bookingId);
                return ServiceResult<BusinessBookingDto>.Failure("Booking not found.", 404);
            }

            if (booking.BusinessId != businessId)
            {
                _logger.LogWarning("Unauthorized attempt to modify booking {BookingId} by business {BusinessId}.", bookingId, businessId);
                return ServiceResult<BusinessBookingDto>.Failure("Access denied.", 403);
            }

            booking.Status = newStatus;
            if (newStatus == BookingStatus.Active)
            {
                booking.ConfirmedAt = DateTimeOffset.UtcNow;
                await _notificationService.SendBookingConfirmationNotification(booking);
            }
            else if (newStatus == BookingStatus.Canceled)
            {
                await _notificationService.SendBookingRejectionNotification(booking);
            }

            await _bookingRepository.UpdateAsync(booking);

            _logger.LogInformation("Booking {BookingId} status updated to '{Status}' by business {BusinessId}.", bookingId, newStatus.ToString(), businessId);

            var dto = _mapper.Map<BusinessBookingDto>(booking);
            return ServiceResult<BusinessBookingDto>.SuccessResult(dto);
        }

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
        public async Task<ServiceResult<UserBookingDto>> UpdateBookingStatusByUserAsync(int bookingId, int userId, BookingStatus newStatus)
        {
            if (bookingId < 0 || userId < 0)
            {
                _logger.LogWarning("Invalid IDs in UpdateBookingStatusByUserAsync. BookingId: {BookingId}, UserId: {UserId}", bookingId, userId);
                return ServiceResult<UserBookingDto>.Failure("Invalid identifiers.", 400);
            }

            if (!Enum.IsDefined(typeof(BookingStatus), newStatus) || newStatus != BookingStatus.Canceled)
            {
                _logger.LogWarning("Invalid booking status value: {Status}", newStatus.ToString());
                return ServiceResult<UserBookingDto>.Failure("Invalid status value.", 400);
            }

            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
            {
                _logger.LogWarning("Booking {BookingId} not found.", bookingId);
                return ServiceResult<UserBookingDto>.Failure("Booking not found.", 404);
            }

            if (booking.UserId != userId)
            {
                _logger.LogWarning("Unauthorized attempt to modify booking {BookingId} by user {UserId}.", bookingId, userId);
                return ServiceResult<UserBookingDto>.Failure("Access denied.", 403);
            }

            booking.Status = newStatus;
            if (newStatus == BookingStatus.Canceled)
            {
                await _notificationService.SendBookingRejectionNotification(booking);
            }

            await _bookingRepository.UpdateAsync(booking);

            _logger.LogInformation("Booking {BookingId} status updated to '{Status}' by user {UserId}.", bookingId, newStatus.ToString(), userId);

            var dto = _mapper.Map<UserBookingDto>(booking);
            return ServiceResult<UserBookingDto>.SuccessResult(dto);
        }

        public async Task<bool> UserHasCompleteBooking(int userId, int businessId)
        {
            return await _bookingRepository.AnyAsync(b => b.UserId == userId && b.BusinessId == businessId && b.Status == BookingStatus.Complete);
        }
    }
}
