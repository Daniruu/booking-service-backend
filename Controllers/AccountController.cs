using BookingService.DTOs;
using BookingService.Extensions;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    /// <summary>
    /// Handles account management actions for authenticated users,
    /// such as changing email address and password.
    /// </summary>
    [ApiController]
    [Route("api/account")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentAccount()
        {
            try
            {
                var accountId = User.GetUserId();

                _logger.LogInformation("Retrieving current account.");
                var result = await _accountService.GetByIdAsync(accountId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to get account: {AccountId}, reason: {Reason}", accountId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogWarning("Account data retrieved successfully: {AccountId}", accountId);
                return Ok(new { accountDto = result.Data });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during retrieval of current account.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Sends a confirmation code to the new email address to verify email change.
        /// </summary>
        /// <param name="dto">New email address.</param>
        /// <returns>Expiration time of the confirmation code or an error message.</returns>
        [HttpPost("email/code")]
        public async Task<IActionResult> RequestChangeEmailCode([FromBody] ChangeEmailRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state while requesting email change code.");
                return BadRequest(ModelState);
            }

            try
            {
                var userId = User.GetUserId();

                _logger.LogInformation("User {UserId} requested email change to: {NewEmail}", userId, dto.NewEmail);
                var result = await _accountService.RequestEmailChangeCodeAsync(userId, dto.NewEmail);

                if (!result.Success)
                {
                    _logger.LogWarning("Email change code request failed for User {UserId}: {Reason}", userId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Email change code sent to {NewEmail} for User {UserId}", dto.NewEmail, userId);
                return Ok(new { expiresAt = result.Data });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during email code request.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Changes the email address after verifying the confirmation code.
        /// </summary>
        /// <param name="dto">New email and the verification code.</param>
        /// <returns>204 No Content if successful, or an error message.</returns>
        [HttpPatch("email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto dto)
        {
            try
            {
                var userId = User.GetUserId();

                _logger.LogInformation("User {UserId} attempting to change email to {NewEmail}", userId, dto.NewEmail);
                var result = await _accountService.ChangeEmailAsync(userId, dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Email change failed for User {UserId}: {Reason}", userId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Email successfully changed for User {UserId} to {NewEmail}", userId, dto.NewEmail);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during email change.");
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Changes the account password.
        /// </summary>
        /// <param name="dto">Current password and new password.</param>
        /// <returns>204 No Content if successful, or an error message.</returns>
        [HttpPatch("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            try
            {
                var userId = User.GetUserId();

                _logger.LogInformation("User {UserId} attempting to change password", userId);
                var result = await _accountService.ChangePasswordAsync(userId, dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Password change failed for User {UserId}: {Reason}", userId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Пароль успешно обновлён");
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during password change.");
                return Unauthorized(new { message = ex.Message });
            }
        }

    }
}
