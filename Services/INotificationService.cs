using BookingService.Models;

namespace BookingService.Services
{
    public interface INotificationService
    {
        Task SendBookingRequestNotification(Booking booking);
        Task SendBookingConfirmationNotification(Booking booking);
        Task SendBookingRejectionNotification(Booking booking);
    }
}
