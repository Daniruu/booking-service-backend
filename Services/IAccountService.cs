using BookingService.DTOs;
using BookingService.Utils;

namespace BookingService.Services
{
    /// <summary>
    /// Service responsible for managing account-related actions for authenticated users,
    /// such as changing email and password.
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Sends a confirmation code to the new email address to verify the email change.
        /// </summary>
        /// <param name="accountId">ID of the authenticated account requesting the change.</param>
        /// <param name="newEmail">New email address to which the confirmation code should be sent.</param>
        /// <returns>Expiration time of the confirmation code or an error.</returns>
        Task<ServiceResult<DateTimeOffset>> RequestEmailChangeCodeAsync(int accountId, string newEmail);

        /// <summary>
        /// Changes the account email address after confirming the verification code.
        /// </summary>
        /// <param name="accountId">ID of the authenticated account.</param>
        /// <param name="dto">New email address and confirmation code.</param>
        /// <returns>Success or failure result.</returns>
        Task<ServiceResult> ChangeEmailAsync(int accountId, ChangeEmailDto dto);
        Task<ServiceResult> ChangePasswordAsync(int accountId, ChangePasswordDto dto);
    }
}
