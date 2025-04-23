using BookingService.DTOs;
using BookingService.Extensions;
using BookingService.Models;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    /// <summary>
    /// Handles employee working schedule.
    /// </summary>
    [ApiController]
    [Route("api/employees/{employeeId}/schedule")]
    [Authorize(Roles = "Business")]
    public class EmployeeScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly ILogger<EmployeeScheduleController> _logger;

        public EmployeeScheduleController(IScheduleService scheduleService, ILogger<EmployeeScheduleController> logger)
        {
            _scheduleService = scheduleService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the weekly schedule of the employee.
        /// </summary>
        /// <returns>
        /// 200 OK with schedule data;
        /// 401 Unauthorized;
        /// 404 Not Found if business doesn't exist.
        /// </returns>
        /// <response code="200">Schedule retrieved</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Employee not found</response>
        [HttpGet]
        public async Task<IActionResult> GetEmployeeSchedule(int employeeId)
        {
            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Getting schedule for employee {EmployeeId}", employeeId);
                var result = await _scheduleService.GetEmployeeScheduleAsync(employeeId, businessId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to get schedule for employee {EmployeeId}: {Reason}", businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return Ok(result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during retrieval of employee schedule.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Replaces the entire weekly schedule of the employee.
        /// </summary>
        /// <param name="schedule">A list of weekly schedule entries.</param>
        /// <returns>
        /// 204 No Content if updated successfully;
        /// 400 Bad Request if invalid;
        /// 401 Unauthorized;
        /// 404 Not Found if employee or business doesn't exist.
        /// </returns>
        /// <response code="204">Schedule updated</response>
        /// <response code="400">Invalid data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Business not found</response>
        /// <response code="404">Employee not found</response>
        [HttpPut]
        public async Task<IActionResult> UpdateEmployeeSchedule(int employeeId, [FromBody] List<DayScheduleUpdateDto> schedule)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in UpdateEmployeeSchedule.");
                return BadRequest(ModelState);
            }

            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Updating schedule for employee {EmployeeId}", employeeId);
                var result = await _scheduleService.UpdateEmployeeScheduleAsync(employeeId, businessId, schedule);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to update schedule for employee {EmployeeId}: {Reason}", employeeId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during update of employee schedule.");
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
