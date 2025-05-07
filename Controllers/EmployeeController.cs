using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookingService.DTOs;
using BookingService.Services;
using BookingService.Models;
using BookingService.Extensions;

namespace BookingService.Controllers
{
    /// <summary>
    /// Handles employee management for the currently authenticated business,
    /// including creation, updating, deletion and avatar handling.
    /// </summary>
    [ApiController]
    [Route("api/businesses/me/employees")]
    [Authorize(Roles = "Business")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;
        public EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves employee list.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Retrieving employee list for business: {BusinessId}", businessId);
                var result = await _employeeService.GetByBusinessIdAsync(businessId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to retrieve employee: {Reason}", result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return Ok(new { employeeDtos = result.Data });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a specific employee by ID.
        /// </summary>
        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployeeById(int employeeId)
        {
            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Retrieving employee {EmployeeId}", employeeId);
                var result = await _employeeService.GetByIdAsync(employeeId, businessId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to retrieve employee: {Reason}", result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return Ok(new { employeeDto = result.Data });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Adds a new employee to the current business.
        /// </summary>
        /// <param name="dto">Employee data.</param>
        /// <returns>
        /// 200 OK with the newly created employee;
        /// 400 Bad Request if validation fails;
        /// 401 Unauthorized if the user is not authenticated;
        /// 500 Internal Server Error if creation fails.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] CreateEmployeeDto dto)
        {
            if (!ModelState.IsValid) 
            {
                _logger.LogWarning("Invalid model state in AddEmployee.");
                return BadRequest(ModelState);
            }

            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Adding new employee.");
                var result = await _employeeService.AddEmployeeAsync(businessId, dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to add employee: {Reason}", result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return CreatedAtAction(nameof(GetEmployeeById), new { employeeId = result.Data.Id }, result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates information of an existing employee.
        /// </summary>
        /// <param name="employeeId">The ID of the employee to update.</param>
        /// <param name="dto">The updated employee data.</param>
        /// <returns>
        /// 200 OK with the updated employee;
        /// 400 Bad Request if validation fails;
        /// 401 Unauthorized if not authenticated;
        /// 404 Not Found if the employee does not exist.
        /// </returns>
        [HttpPut("{employeeId}")]
        public async Task<IActionResult> UpdateEmployee(int employeeId, [FromBody] UpdateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in UpdateEmployee.");
                return BadRequest(ModelState);
            }

            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Updating employee {EmployeeId}", employeeId);
                var result = await _employeeService.UpdateEmployeeAsync(employeeId, businessId, dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to update employee {EmployeeId}: {Reason}", employeeId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return Ok(new { employeeDto = result.Data });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during update of employee data.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes an employee from the business.
        /// </summary>
        /// <param name="employeeId">The ID of the employee to delete.</param>
        /// <returns>
        /// 204 No Content if deletion was successful;
        /// 401 Unauthorized if not authenticated;
        /// 403 Forbidden if the employee does not belong to the business;
        /// 404 Not Found if the employee does not exist.
        /// </returns>
        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Deleting employee {EmployeeId}", employeeId);
                var result = await _employeeService.DeleteEmployeeAsync(employeeId, businessId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to delete employee {EmployeeId}: {Reason}", employeeId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }
                
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt to delete employee.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Uploads a new avatar for the employee.
        /// </summary>
        /// <remarks>
        /// Accepts image files in .jpg, .jpeg, or .png format. The file size must not exceed 5 MB.
        /// </remarks>
        /// <param name="employeeId">ID of the employee for whom the avatar is being uploaded.</param>
        /// <param name="file">The image file to upload.</param>
        /// <returns>
        /// Returns 200 OK with the new avatar URL if successful;
        /// 400 Bad Request if the file is missing, too large, or in an unsupported format;
        /// 401 Unauthorized;
        /// 404 Not Found if the employee does not exist;
        /// 500 Internal Server Error for unexpected issues.
        /// </returns>
        [HttpPost("{employeeId}/avatar")]
        public async Task<IActionResult> UploadAvatar(int employeeId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided." });

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = "Max file size is 5 MB." });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(ext))
                return BadRequest(new { message = "Unsupported file format." });

            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Uploading avatar for employee {EmployeeId}", employeeId);
                var result = await _employeeService.UploadAvatarAsync(employeeId, businessId, file);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to upload avatar for employee {EmployeeId}: {Reason}", employeeId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return Ok(new { avatarUrl = result.Data });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during employee avatar upload.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes the avatar of a specific employee.
        /// </summary>
        /// <returns>
        /// <param name="employeeId">ID of the employee for whom the avatar is being removed.</param>
        /// 204 No Content if successful;
        /// 404 Not Found if the employee or avatar does not exist;
        /// 401 Unauthorized if the user is not authenticated;
        /// 500 Failed to delete employee avatar from storage;
        /// </returns>
        [HttpDelete("{employeeId}/avatar")]
        public async Task<IActionResult> DeleteAvatar(int employeeId)
        {
            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Deleting avatar for employee {EmployeeId}", employeeId);
                var result = await _employeeService.DeleteAvatarAsync(employeeId, businessId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to delete avatar for employee {EmployeeId}: {Reason}", employeeId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during employee avatar remove.");
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
