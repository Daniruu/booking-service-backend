using BookingService.DTOs;
using BookingService.Extensions;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    /// <summary>
    /// Handles business working schedule for the currently authenticated business.
    /// </summary>
    [ApiController]
    [Route("api/businesses/me/schedule")]
    [Authorize(Roles = "Business")]
    public class BusinessScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly ILogger<BusinessScheduleController> _logger;

        public BusinessScheduleController(IScheduleService scheduleService, ILogger<BusinessScheduleController> logger)
        {
            _scheduleService = scheduleService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the weekly schedule of the current business.
        /// </summary>
        /// <returns>
        /// 200 OK with schedule data;
        /// 401 Unauthorized;
        /// 404 Not Found if business doesn't exist.
        /// </returns>
        /// <response code="200">Schedule retrieved</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Business not found</response>
        [HttpGet]
        public async Task<IActionResult> GetBusinessSchedule()
        {
            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Getting schedule for business {BusinessId}", businessId);
                var result = await _scheduleService.GetBusinessScheduleAsync(businessId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to get schedule for business {BusinessId}: {Reason}", businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return Ok(new { scheduleDto = result.Data });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during retrieval of current business schedule.");
                return Unauthorized(new { message = ex.Message });
            }
        }


        /// <summary>
        /// Replaces the entire weekly schedule of the current business.
        /// </summary>
        /// <param name="schedule">A list of weekly schedule entries.</param>
        /// <returns>
        /// 204 No Content if updated successfully;
        /// 400 Bad Request if invalid;
        /// 401 Unauthorized;
        /// 404 Not Found if business doesn't exist.
        /// </returns>
        /// <response code="204">Schedule updated</response>
        /// <response code="400">Invalid data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Business not found</response>
        [HttpPut]
        public async Task<IActionResult> UpdateBusinessSchedule([FromBody] List<UpdateDayScheduleDto> schedule)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in UpdateBusinessSchedule.");
                return BadRequest(ModelState);
            }

            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Updating schedule for business {BusinessId}", businessId);
                var result = await _scheduleService.UpdateBusinessScheduleAsync(businessId, schedule);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to update schedule for business {BusinessId}: {Reason}", businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during update of current business schedule.");
                return Unauthorized(new { message = ex.Message });
            }
        }

    }
}
