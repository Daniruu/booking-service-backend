using BookingService.Data;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Utils;

namespace BookingService.Services
{
    /// <summary>
    /// Handles password recovery logic.
    /// </summary>
    public class PasswordRecoveryService : IPasswordRecoveryService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConfirmationCodeService _confirmationCodeService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly BookingServiceDbContext _context;
        private readonly ILogger<PasswordRecoveryService> _logger;
        public PasswordRecoveryService(
            IAccountRepository accountRepository,
            IConfirmationCodeService confirmationCodeService,
            IPasswordHasher passwordHasher,
            BookingServiceDbContext context,
            ILogger<PasswordRecoveryService> logger)
        {
            _accountRepository = accountRepository;
            _confirmationCodeService = confirmationCodeService;
            _passwordHasher = passwordHasher;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Resets the password for the specified account after verifying the confirmation code.
        /// </summary>
        /// <param name="dto">Data containing email, new password, and the confirmation code.</param>
        /// <returns>Success or error result.</returns>
        public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto dto)
        {
            _logger.LogInformation("Attempting password reset for {Email}", dto.Email);

            dto.Email = dto.Email.Trim().ToLower();

            var verification = await _confirmationCodeService.VerifyAsync(dto.Email, dto.Code, ConfirmationCodeType.PasswordRecovery);
            if (!verification.Success)
            {
                _logger.LogWarning("Password reset verification failed for {Email}. Reason: {Reason}", dto.Email, verification.ErrorMessage);
                return ServiceResult.Failure(verification.ErrorMessage!, verification.StatusCode);
            }

            var account = await _accountRepository.GetByEmailAsync(dto.Email);
            if (account == null)
            {
                _logger.LogWarning("Account not found during password reset: {Email}", dto.Email);
                return ServiceResult.Failure("Account not found.", 404);
            }

            
            
            account.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);

            await _accountRepository.Update(account);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Password successfully reset for {Email}", dto.Email);
            return ServiceResult.SuccessResult();
        }
    }
}
