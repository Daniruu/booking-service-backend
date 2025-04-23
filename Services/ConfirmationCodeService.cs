using BookingService.Data;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Utils;
using Microsoft.Extensions.Logging;

namespace BookingService.Services
{
    /// <summary>
    /// Service responsible for generating and verifying confirmation codes for actions like email verification,
    /// password recovery, and email change.
    /// </summary>
    public class ConfirmationCodeService : IConfirmationCodeService
    {
        private readonly BookingServiceDbContext _context;
        private readonly IConfirmationCodeRepository _codeRepository;
        private readonly IEmailSender _emailSender;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<ConfirmationCodeService> _logger;

        public ConfirmationCodeService(
            BookingServiceDbContext context,
            IConfirmationCodeRepository codeRepository, 
            IEmailSender emailSender, 
            IAccountRepository accountRepository,
            ILogger<ConfirmationCodeService> logger)
        {
            _context = context;
            _codeRepository = codeRepository;
            _emailSender = emailSender;
            _accountRepository = accountRepository;
            _logger = logger;
        }

        /// <summary>
        /// Generates a new confirmation code (or updates an existing one) and sends it to the specified email address.
        /// </summary>
        /// <param name="email">Target email address.</param>
        /// <param name="type">Type of confirmation code.</param>
        /// <returns>Expiration time of the generated code or an error.</returns>
        public async Task<ServiceResult<DateTimeOffset>> GenerateAndSendAsync(string email, ConfirmationCodeType type)
        {
            _logger.LogInformation("Generating confirmation code: {Email}, Type: {Type}", email, type);

            email = email.Trim().ToLower();

            switch (type)
            {
                case ConfirmationCodeType.EmailConfirmaiton:
                    if (await _accountRepository.EmailExistsAsync(email))
                    {
                        _logger.LogWarning("Email already registered: {Email}", email);
                        return ServiceResult<DateTimeOffset>.Failure("Email is already registered.", 409);
                    }
                    break;

                case ConfirmationCodeType.PasswordRecovery:
                case ConfirmationCodeType.EmailChange:
                    if (!await _accountRepository.EmailExistsAsync(email))
                    {
                        _logger.LogWarning("Email not found for action {Type}: {Email}", type, email);
                        return ServiceResult<DateTimeOffset>.Failure("Email not found.", 404);
                    }
                    break;
            }

            var existingCode = await _codeRepository.GetByEmailAndTypeAsync(email, type);
            if (existingCode != null && DateTimeOffset.UtcNow < existingCode.LastRequestTime.AddMinutes(1))
            {
                _logger.LogWarning("Confirmation code request too frequent for: {Email}", email);
                return ServiceResult<DateTimeOffset>.Failure("You can request a new code after 1 minute.", 429);
            }

            var confirmationCode = existingCode ?? new ConfirmationCode
            {
                Email = email,
                CodeType = type,
                Code = ConfirmationCode.Generate(),
                ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(15),
                LastRequestTime = DateTimeOffset.UtcNow
            };

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (existingCode == null)
                {
                    await _codeRepository.Add(confirmationCode);
                    _logger.LogInformation("Confirmation code created: {Email}, Type: {Type}", email, type);
                }
                else
                {
                    confirmationCode.LastRequestTime = DateTimeOffset.UtcNow;
                    confirmationCode.Code = ConfirmationCode.Generate();
                    confirmationCode.ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(15);
                    await _codeRepository.Update(confirmationCode);
                    _logger.LogInformation("Confirmation code updated: {Email}, Type: {Type}", email, type);
                }

                await _context.SaveChangesAsync();

                var templateKey = GetEmailTemplate(type);
                var dynamicData = new Dictionary<string, string> { { "code", confirmationCode.Code } };
                await _emailSender.SendEmailWithTemplateAsync(email, templateKey, dynamicData);

                await transaction.CommitAsync();

                return ServiceResult<DateTimeOffset>.SuccessResult(confirmationCode.ExpirationTime);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Failed to generate/send confirmation code: {Email}, Type: {Type}", email, type);
                return ServiceResult<DateTimeOffset>.Failure("Failed to send confirmation code.", 500);
            }
        }

        /// <summary>
        /// Verifies the confirmation code provided for a given email and action type.
        /// </summary>
        /// <param name="email">Email address to verify.</param>
        /// <param name="code">Code entered by the user.</param>
        /// <param name="type">Type of confirmation code.</param>
        /// <returns>Success or failure result.</returns>
        public async Task<ServiceResult> VerifyAsync(string email, string code, ConfirmationCodeType type)
        {
            _logger.LogInformation("Verifying confirmation code for {Email}, Type: {Type}", email, type);

            var confirmationCode = await _codeRepository.GetByEmailAndTypeAsync(email, type);

            if (confirmationCode == null || confirmationCode.Code != code || confirmationCode.ExpirationTime < DateTimeOffset.UtcNow)
            {
                _logger.LogWarning("Invalid or expired code: {Email}, Type: {Type}", email, type);
                return ServiceResult.Failure("Invalid or expired confirmation code.", 400);
            }

            _logger.LogInformation("Confirmation code verified for {Email}, Type: {Type}", email, type);
            return ServiceResult.SuccessResult();
        }

        /// <summary>
        /// Returns the appropriate email template ID for the given confirmation code type.
        /// </summary>
        private static string GetEmailTemplate(ConfirmationCodeType type)
        {
            return type switch
            {
                ConfirmationCodeType.EmailConfirmaiton => "d-237e5ebc882040afa0becb95384d8287",
                ConfirmationCodeType.PasswordRecovery => "d-7ad76a06999b49c5a9385f8b0be67d89",
                ConfirmationCodeType.EmailChange => "d-bb28d22fe5a94bc18a77fc1871a84560",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}
