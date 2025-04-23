using BookingService.Models;
using BookingService.Utils;

namespace BookingService.Services
{
    /// <summary>
    /// Service responsible for generating and verifying confirmation codes for actions like email verification,
    /// password recovery, and email change.
    /// </summary>
    public interface IConfirmationCodeService
    {
        /// <summary>
        /// Generates a new confirmation code (or updates an existing one) and sends it to the specified email address.
        /// </summary>
        /// <param name="email">Target email address.</param>
        /// <param name="type">Type of confirmation code.</param>
        /// <returns>Expiration time of the generated code or an error.</returns>
        Task<ServiceResult<DateTimeOffset>> GenerateAndSendAsync(string email, ConfirmationCodeType type);

        /// <summary>
        /// Verifies the confirmation code provided for a given email and action type.
        /// </summary>
        /// <param name="email">Email address to verify.</param>
        /// <param name="code">Code entered by the user.</param>
        /// <param name="type">Type of confirmation code.</param>
        /// <returns>Success or failure result.</returns>
        Task<ServiceResult> VerifyAsync(string email, string code, ConfirmationCodeType type);
    }
}
