using BookingService.DTOs;
using BookingService.Extensions;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    /// <summary>
    /// Handles operations related to the business gallery images for the authenticated business.
    /// </summary>
    [ApiController]
    [Route("api/businesses/me/images")]
    [Authorize(Roles = "Business")]
    public class BusinessImageController : ControllerBase
    {
        private readonly IBusinessImageService _businessImageService;
        private readonly ILogger<BusinessImageController> _logger;

        public BusinessImageController(IBusinessImageService businessImageService, ILogger<BusinessImageController> logger)
        {
            _businessImageService = businessImageService;
            _logger = logger;
        }

        /// <summary>
        /// Uploads a new image to the business gallery.
        /// </summary>
        /// <param name="dto">DTO containing the image file and optional metadata.</param>
        /// <returns>
        /// 200 OK with uploaded image info;
        /// 400 Bad Request for invalid file;
        /// 401 Unauthorized;
        /// 500 Internal Server Error for unexpected issues.
        /// </returns>
        /// <response code="200">Image uploaded successfully</response>
        /// <response code="400">Invalid image file</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] BusinessImageUploadDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
            {
                _logger.LogWarning("Image upload failed: No file provided.");
                return BadRequest(new { message = "No file was provided." });
            }

            if (dto.File.Length > 5 * 1024 * 1024)
            {
                _logger.LogWarning("Image upload failed: File is large ({Size} bytes)", dto.File.Length);
                return BadRequest(new { messge = "The file is too large. Max: 5 MB. " });
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(dto.File.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                _logger.LogWarning("Image upload failed: Unsupported file format {Extension}", extension);
                return BadRequest(new { message = "Unsupported file format. Allowed formats are .jpg, .jpeg, .png." });
            }

            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Uploading image for busines {BusinessId}", businessId);
                var result = await _businessImageService.UploadImageAsync(businessId, dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to upload image for business {BusinessId}: {Reason}", businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return Ok(result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during image upload.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes an image from the business gallery.
        /// </summary>
        /// <param name="imageId">The ID of the image to delete.</param>
        /// <returns>
        /// 204 No Content if successful;
        /// 404 if image not found;
        /// 401 Unauthorized.
        /// </returns>
        /// <response code="204">Image deleted</response>
        /// <response code="404">Image not found</response>
        /// <response code="401">Unauthorized</response>
        [HttpDelete("{imageId}")]
        public async Task<IActionResult> Delete(int imageId)
        {
            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Deleting image {ImageId} for business {BusinessId}", imageId, businessId);
                var result = await _businessImageService.DeleteImageAsync(businessId, imageId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to delete image {ImageId} for business {BusinessId}: {Reason}", imageId, businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during image remove.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Sets the specified business image as the primary image.
        /// </summary>
        /// <param name="imageId">The ID of the image to set as primary.</param>
        /// <returns>
        /// 204 No Content if successful;
        /// 404 if image not found;
        /// 401 Unauthorized.
        /// </returns>
        /// <response code="204">Primary image set</response>
        /// <response code="404">Image not found</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("{imageId}/set-primary")]
        public async Task<IActionResult> SetPrimary(int imageId)
        {
            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Setting image {ImageId} as primary for business {BusinessId}", imageId, businessId);
                var result = await _businessImageService.SetPrimaryImageAsync(businessId, imageId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to set primary image {ImageId} for business {BusinessId}: {Reason}", imageId, businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access.");
                return Unauthorized(new { message = ex.Message });
            }
        }

    }
}
