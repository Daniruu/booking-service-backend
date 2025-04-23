using BookingService.DTOs;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    /// <summary>
    /// Handles authentication-related actions such as login, token refresh, and logout.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates the user and returns an access token.
        /// A refresh token is stored in a secure HTTP-only cookie.
        /// </summary>
        /// <param name="dto">Login credentials including email and password.</param>
        /// <returns>Access token or an error message.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state on login attempt.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Login attempt for: {Email}", dto.Email);
            var result = await _authService.AuthenticateUserAsync(dto.Email, dto.Password);

            if (!result.Success)
            {
                _logger.LogWarning("Login failed for: {Email}, reason: {Reason}", dto.Email, result.ErrorMessage);
                return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
            }

            _logger.LogInformation("Login successful for: {Email}", dto.Email);
            return Ok(new { accessToken = result.Data });
        }

        /// <summary>
        /// Refreshes the access token using a refresh token stored in a cookie.
        /// </summary>
        /// <returns>New access token or an error message.</returns>
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["RefreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Refresh token not found in cookie.");
                return Unauthorized(new { message = "Refresh token is missing." });
            }

            _logger.LogInformation("Refreshing token using refresh token: {Token}", refreshToken);
            var result = await _authService.RefreshTokenAsync(refreshToken);

            if (!result.Success)
            {
                _logger.LogWarning("Token refresh failed: {Reason}", result.ErrorMessage);
                return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
            }

            _logger.LogInformation("Token refreshed successfully.");
            return Ok(new { accessToken = result.Data });
        }

        /// <summary>
        /// Logs the user out by invalidating the refresh token and clearing the cookie.
        /// </summary>
        /// <returns>204 No Content if successful or an error message.</returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["RefreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Logout failed: refresh token not found.");
                return Unauthorized(new { message = "Refresh token not found." });
            }

            _logger.LogInformation("Logout attempt using refresh token: {Token}", refreshToken);
            var result = await _authService.LogoutAsync(refreshToken);

            if (!result.Success)
            {
                _logger.LogWarning("Logout failed: {Reason}", result.ErrorMessage);
                return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
            }

            _logger.LogInformation("Logout successful.");
            return NoContent();
        }
    }
}
