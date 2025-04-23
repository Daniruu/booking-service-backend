using BookingService.DTOs;
using BookingService.Utils;

namespace BookingService.Services
{
    /// <summary>
    /// Handles password recovery logic.
    /// </summary>
    public interface IPasswordRecoveryService
    {
        /// <summary>
        /// Resets the password for the specified account after verifying the confirmation code.
        /// </summary>
        /// <param name="dto">Data containing email, new password, and the confirmation code.</param>
        /// <returns>Success or error result.</returns>
        Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto dto);
    }
}
