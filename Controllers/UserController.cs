using BookingService.DTOs;
using BookingService.Extensions;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    /// <summary>
    /// Provides endpoints related to the current authenticated user.
    /// </summary>
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the profile information of the currently authenticated user.
        /// </summary>
        /// <returns>
        /// Returns 200 OK with user profile data if successful;
        /// 401 Unauthorized if the user is not authenticated;
        /// 404 Not Found if the user does not exist;
        /// </returns>
        /// <response code="200">Returns the user profile</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="404">User not found</response>
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = User.GetUserId();

                _logger.LogInformation("Retrieving current user info.");
                var result = await _userService.GetByIdAsync(userId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to get user: {UserId}, reason: {Reason}", userId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return Ok(result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during retrieval of current user.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the current user's name and/or surname.
        /// </summary>
        /// <remarks>
        /// This endpoint allows the authenticated user to update their personal information,
        /// such as name and surname. Only the fields included in the request will be updated.
        /// </remarks>
        /// <param name="dto">Object containing the updated name and/or surname.</param>
        /// <returns>
        /// 204 No Content if the update was successful; 
        /// 400 Bad Request if the input is invalid; 
        /// 401 Unauthorized if the user is not authenticated;
        /// 404 Not Found if the user does not exist.
        /// </returns>
        /// <response code="204">User data updated successfully</response>
        /// <response code="400">Invalid input</response>
        /// <response code="401">Unauthorized access</response>
        /// <response code="404">User not found</response>
        [HttpPatch("me")]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UserUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in UpdateUser.");
                return BadRequest(ModelState);
            }

            if (dto.Name == null && dto.Surname == null)
            {
                _logger.LogWarning("User update attempt with empty payload.");
                return BadRequest(new { message = "At least one field (name or surname) must be provided." });
            }

            try
            {
                var userId = User.GetUserId();

                _logger.LogInformation("Updating current user data.");
                var result = await _userService.UpdateUserAsync(userId, dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to update user: {UserId}, reason: {Reason}", userId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogInformation("User {UserId} successfully updated their profile.", userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during update of current user data.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Uploads a new avatar for the authenticated user. Replaces the previous avatar if it exists.
        /// </summary>
        /// <remarks>
        /// Accepts image files in .jpg, .jpeg, or .png format. The file size must not exceed 5 MB.
        /// </remarks>
        /// <param name="file">The image file to upload.</param>
        /// <returns>
        /// Returns 200 OK with the new avatar URL if successful;
        /// 400 Bad Request if the file is missing, too large, or in an unsupported format;
        /// 401 Unauthorized if the user is not authenticated;
        /// 500 Internal Server Error for unexpected issues.
        /// </returns>
        /// <response code="200">Avatar uploaded successfully</response>
        /// <response code="400">Invalid file</response>
        /// <response code="401">User is not authenticated</response>
        [HttpPost("me/avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Avatar upload failed: No file provided.");
                return BadRequest(new { message = "No file was provided." });
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                _logger.LogWarning("Avatar upload failed: File too large ({Size} bytes)", file.Length);
                return BadRequest(new { message = "The file is too large. Maximum allowed size is 5 MB." });
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                _logger.LogWarning("Avatar upload failed: Unsupported file format {Extension}", extension);
                return BadRequest(new { message = "Unsupported file format. Allowed formats are .jpg, .jpeg, .png." });
            }

            try
            {
                var userId = User.GetUserId();
                _logger.LogInformation("Uploading new avatar for user {UserId}", userId);

                var result = await _userService.UploadAvatarAsync(userId, file);

                if (!result.Success)
                {
                    _logger.LogWarning("Avatar upload failed for user {UserId}: {Reason}", userId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Avatar uploaded successfully for user {UserId}", userId);
                return Ok(new { avatarUrl = result.Data });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during avatar upload.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes the current user's avatar from the storage and unlinks it from the profile.
        /// </summary>
        /// <returns>
        /// 204 No Content if successful;
        /// 404 Not Found if the user or avatar does not exist;
        /// 401 Unauthorized if the user is not authenticated;
        /// </returns>
        [HttpDelete("me/avatar")]
        public async Task<IActionResult> DeleteAvatar()
        {
            try
            {
                var userId = User.GetUserId();

                _logger.LogInformation("User {UserId} requested avatar deletion", userId);
                var result = await _userService.DeleteAvatarAsync(userId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to delete avatar for user {UserId}: {Reason}", userId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Avatar deleted successfully for user {UserId}", userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during avatar deletion.");
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
