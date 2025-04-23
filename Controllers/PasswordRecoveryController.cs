using BookingService.DTOs;
using BookingService.Models;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    /// <summary>
    /// Handles password recovery process including sending a recovery code and resetting the password.
    /// </summary>
    [ApiController]
    [Route("api/password-recoveries")]
    [AllowAnonymous]
    public class PasswordRecoveryController : ControllerBase
    {
        private readonly IConfirmationCodeService _confirmationCodeService;
        private readonly IPasswordRecoveryService _passwordRecoveryService;
        private readonly ILogger<PasswordRecoveryController> _logger;
        public PasswordRecoveryController(
            IConfirmationCodeService confirmationCodeService, 
            IPasswordRecoveryService passwordRecoveryService, 
            ILogger<PasswordRecoveryController> logger)
        {
            _confirmationCodeService = confirmationCodeService;
            _passwordRecoveryService = passwordRecoveryService;
            _logger = logger;
        }

        /// <summary>
        /// Sends a password recovery confirmation code to the specified email.
        /// </summary>
        /// <param name="dto">Email address for which the recovery code should be sent.</param>
        /// <returns>Expiration time of the generated code or an error message.</returns>
        [HttpPost]
        public async Task<IActionResult> RequestRecoveryCode([FromBody] PasswordRecoveryRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state while requesting password recovery code.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Requesting password recovery code for email: {Email}", dto.Email);
            var result = await _confirmationCodeService.GenerateAndSendAsync(dto.Email, ConfirmationCodeType.PasswordRecovery);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to send password recovery code to: {Email}, reason: {Reason}", dto.Email, result.ErrorMessage);
                return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
            }

            _logger.LogInformation("Password recovery code sent successfully to: {Email}", dto.Email);
            return Ok(new { expiresAt = result.Data });
        }

        /// <summary>
        /// Resets the user's password using a previously sent recovery code.
        /// </summary>
        /// <param name="dto">Password reset data including email, new password, and confirmation code.</param>
        /// <returns>204 No Content if successful or an error message.</returns>
        [HttpPatch]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for password reset.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Attempting password reset for: {Email}", dto.Email);
            var result = await _passwordRecoveryService.ResetPasswordAsync(dto);

            if (!result.Success)
            {
                _logger.LogWarning("Password reset failed for: {Email}, reason: {Reason}", dto.Email, result.ErrorMessage);
                return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
            }

            _logger.LogInformation("Password reset successfully for: {Email}", dto.Email);
            return NoContent();
        }
    }
}
