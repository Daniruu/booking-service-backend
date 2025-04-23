using BookingService.Models;
using BookingService.Repositories;

namespace BookingService.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailSender _emailSender;
        private readonly IAccountRepository _accountRepository;
        
        public NotificationService(IEmailSender emailSender, IAccountRepository accountRepository)
        {
            _emailSender = emailSender;
            _accountRepository = accountRepository;
        }

        public async Task SendBookingRequestNotification(Booking booking)
        {
            var businessOwner = await _accountRepository.GetByIdAsync(booking.BusinessId);
            if (businessOwner == null)
                return;

            string subject = "New Booking Request";
            string message = $"You have a new booking request for {booking.Service.Name} on {booking.StartTime}. Please confirm or reject.";

            await _emailSender.SendEmailAsync(businessOwner.Email, subject, message);
        }

        public async Task SendBookingConfirmationNotification(Booking booking)
        {
            var user = await _accountRepository.GetByIdAsync(booking.UserId);
            if (user == null)
                return; //Может лучше бросать ошибку, а не возвращать "ничего"?

            string subject = "Your Booking is Confirmed";
            string message = $"You booking for {booking.Service.Name} on {booking.StartTime} has been confirmed.";

            await _emailSender.SendEmailAsync(user.Email, subject, message);
        }

        public async Task SendBookingRejectionNotification(Booking booking)
        {
            var user = await _accountRepository.GetByIdAsync(booking.UserId);
            if (user == null)
                return;

            string subject = "Your Booking is Rejected";
            string message = $"Your booking for {booking.Service.Name} on {booking.StartTime} was rejected.";

            await _emailSender.SendEmailAsync(user.Email, subject, message);
        }


    }
}
