using BookingService.DTOs;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new business category with an associated icon image.
        /// </summary>
        /// <param name="dto">The data for the new category including the name and the icon file.</param>
        /// <returns>
        /// 200 OK with the ID of the created category;
        /// 400 Bad Request if the input data or file is invalid;
        /// 500 Internal Server Error on failure.
        /// </returns>
        /// <response code="200">Category created successfully.</response>
        /// <response code="400">Invalid input or unsupported file.</response>
        /// <response code="500">Server error while creating the category.</response>
        [HttpPost("categories")]
        public async Task<IActionResult> AddBusinessCategory([FromForm] BusinessCategoryCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in AddBusinessCategory.");
                return BadRequest(ModelState);
            }

            if (dto.Icon == null || dto.Icon.Length == 0)
            {
                _logger.LogWarning("Missing icon file in AddBusinessCategory.");
                return BadRequest(new { message = "Icon file is required." });
            }

            if (dto.Icon.Length > 5 * 1024 * 1024)
            {
                _logger.LogWarning("Icon file too large: {Size} bytes", dto.Icon.Length);
                return BadRequest(new { message = "The file is too large. Maximum size is 5 MB." });
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(dto.Icon.FileName);
            if (!allowedExtensions.Contains(extension.ToLower()))
            {
                _logger.LogWarning("Unsupported file format: {Extension}", extension);
                return BadRequest(new { message = "Unsupported file format. Only JPG and PNG are allowed." });
            }

            try
            {
                _logger.LogInformation("Admin attempting to add new business category: {Name}", dto.Name);
                var result = await _adminService.AddBusinessCategoryAsync(dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to add category: {Reason}", result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Successfully created category ID {CategoryId}", result.Data);
                return Ok(new { categoryId = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating business category.");
                return StatusCode(500, new { message = "Internal server error while creating category." });
            }
        }

    }
}
