using BookingService.DTOs;
using BookingService.Utils;

namespace BookingService.Services
{
    /// <summary>
    /// Handles authentication logic including user login, token refresh, and logout.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user using email and password. If successful, returns an access token and sets a refresh token cookie.
        /// </summary>
        /// <param name="email">User's email address.</param>
        /// <param name="password">User's password.</param>
        /// <returns>Access token or error result.</returns>
        Task<ServiceResult<string>> AuthenticateUserAsync(string email, string password);

        /// <summary>
        /// Refreshes the access token using a valid refresh token from the cookie.
        /// </summary>
        /// <param name="refreshToken">Refresh token stored in HTTP-only cookie.</param>
        /// <returns>New access token or error result.</returns>
        Task<ServiceResult<string>> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Logs the user out by invalidating the refresh token and removing the cookie.
        /// </summary>
        /// <param name="refreshToken">Current refresh token from the cookie.</param>
        /// <returns>Success or error result.</returns>
        Task<ServiceResult> LogoutAsync(string refreshToken);
    }
}
