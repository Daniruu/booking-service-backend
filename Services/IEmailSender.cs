using BookingService.Utils;

namespace BookingService.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string message, string? htmlContent = null);
        Task SendEmailWithTemplateAsync(string toEmail, string templateId, Dictionary<string, string> dynamicData);
    }
}
