using AutoMapper;
using BookingService.Data;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Utils;

namespace BookingService.Services
{
    /// <summary>
    /// Service responsible for managing account-related actions for authenticated users,
    /// such as changing email and password.
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly IConfirmationCodeService _confirmationCodeService;
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly BookingServiceDbContext _context;
        private readonly ILogger<AccountService> _logger;
        private readonly IMapper _mapper;

        public AccountService(
            IConfirmationCodeService confirmationCodeService, 
            IAccountRepository accountRepository, 
            IPasswordHasher passwordHasher,
            BookingServiceDbContext context, 
            ILogger<AccountService> logger,
            IMapper mapper)
        {
            _confirmationCodeService = confirmationCodeService;
            _accountRepository = accountRepository;
            _passwordHasher = passwordHasher;
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ServiceResult<AccountDto>> GetByIdAsync(int accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                _logger.LogWarning("Account not found for ID {AccountId}", accountId);
                return ServiceResult<AccountDto>.Failure("Account not found.", 404);
            }

            return ServiceResult<AccountDto>.SuccessResult(_mapper.Map<AccountDto>(account));
        }

        /// <summary>
        /// Sends a confirmation code to the new email address to verify the email change.
        /// </summary>
        /// <param name="accountId">ID of the authenticated account requesting the change.</param>
        /// <param name="newEmail">New email address to which the confirmation code should be sent.</param>
        /// <returns>Expiration time of the confirmation code or an error.</returns>
        public async Task<ServiceResult<DateTimeOffset>> RequestEmailChangeCodeAsync(int accountId, string newEmail)
        {
            _logger.LogInformation("Requesting email change code for AccountId {AccountId} to {NewEmail}", accountId, newEmail);

            newEmail = newEmail.Trim().ToLower();

            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                _logger.LogWarning("Account not found for ID {AccountId} during email change request", accountId);
                return ServiceResult<DateTimeOffset>.Failure("Account not found.", 404);
            }

            if (await _accountRepository.EmailExistsAsync(newEmail))
            {
                _logger.LogWarning("New email is already in use {Email}", newEmail);
                return ServiceResult<DateTimeOffset>.Failure("Email is already registered.", 409);
            }

            return await _confirmationCodeService.GenerateAndSendAsync(newEmail, ConfirmationCodeType.EmailChange);
        }

        /// <summary>
        /// Changes the account email address after confirming the verification code.
        /// </summary>
        /// <param name="accountId">ID of the authenticated account.</param>
        /// <param name="dto">New email address and confirmation code.</param>
        /// <returns>Success or failure result.</returns>
        public async Task<ServiceResult> ChangeEmailAsync(int accountId, ChangeEmailDto dto)
        {
            _logger.LogInformation("Changing email for AccountId {AccountId} to {NewEmail}", accountId, dto.NewEmail);

            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                _logger.LogWarning("Account not found during email change: ID {AccountId}", accountId);
                return ServiceResult.Failure("Account not found.", 404);
            }

            dto.NewEmail = dto.NewEmail.Trim().ToLower();

            var verification = await _confirmationCodeService.VerifyAsync(dto.NewEmail, dto.Code, ConfirmationCodeType.EmailChange);
            if (!verification.Success)
            {
                _logger.LogWarning("Email change verification failed for AccountId {AccountId}: {Reason}", accountId, verification.ErrorMessage);
                return ServiceResult.Failure(verification.ErrorMessage!, verification.StatusCode);
            }

            if (await _accountRepository.EmailExistsAsync(dto.NewEmail))
            {
                _logger.LogWarning("New email is already in use {Email}", dto.NewEmail);
                return ServiceResult.Failure("Email is already registered.", 409);
            }

            account.Email = dto.NewEmail;
            await _accountRepository.Update(account);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Email changed successfully for AccountId {AccountId}", accountId);
            return ServiceResult.SuccessResult();
        }

        /// <summary>
        /// Changes the account password after verifying the current password.
        /// </summary>
        /// <param name="accountId">ID of the authenticated account.</param>
        /// <param name="dto">Current password and new password.</param>
        /// <returns>Success or failure result.</returns>
        public async Task<ServiceResult> ChangePasswordAsync(int accountId, ChangePasswordDto dto)
        {
            _logger.LogInformation("Changing password for AccountId {AccountId}", accountId);

            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                _logger.LogWarning("Account not found during password change: ID {AccountId}", accountId);
                return ServiceResult.Failure("Account not found.", 404);
            }

            if (!_passwordHasher.VerifyPassword(dto.CurrentPassword, account.PasswordHash))
            {
                _logger.LogWarning("Incorrect current password provided for AccountId {AccountId}", accountId);
                return ServiceResult.Failure("Incorrect current password.", 400);
            }

            var newPasswordHash = _passwordHasher.HashPassword(dto.NewPassword);

            account.PasswordHash = newPasswordHash;
            await _accountRepository.Update(account);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Password changed successfully for AccountId {AccountId}", accountId);
            return ServiceResult.SuccessResult();
        }
    }
}
