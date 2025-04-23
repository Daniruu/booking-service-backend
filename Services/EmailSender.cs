using BookingService.Utils;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BookingService.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message, string? htmlContent = null)
        {
            var apiKey = _configuration["EmailSettings:SendGridApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("SenderGrid API key is not configured.");
            }

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_configuration["EmailSettings:Sender"], "BookItEasy");
            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, htmlContent);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Body.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to send email. Status code: {response.StatusCode}, Response: {errorBody}");
            }
        }

        public async Task SendEmailWithTemplateAsync(string toEmail, string templateId, Dictionary<string, string> dynamicData)
        {
            var apiKey = _configuration["EmailSettings:SendGridApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("SenderGrid API key is not configured.");
            }

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_configuration["EmailSettings:Sender"], "BookItEasy");
            var to = new EmailAddress(toEmail);

            var msg = new SendGridMessage
            {
                From = from,
                TemplateId = templateId,
            };
            msg.AddTo(to);

            msg.SetTemplateData(dynamicData);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Body.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to send email. Status code: {response.StatusCode}, Response: {errorBody}");
            }
        }
    }
}
