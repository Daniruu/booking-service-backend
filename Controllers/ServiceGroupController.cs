using BookingService.DTOs;
using BookingService.Extensions;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    /// <summary>
    /// Manages service groups for the currently authenticated business account,
    /// including creation, update, deletion, and reordering.
    /// </summary>
    [ApiController]
    [Route("api/businesses/me/service-groups")]
    [Authorize(Roles = "Business")]
    public class ServiceGroupController : ControllerBase
    {
        private readonly IServiceGroupService _serviceGroupService;
        private readonly ILogger<ServiceGroupController> _logger;

        public ServiceGroupController(IServiceGroupService serviceGroupService, ILogger<ServiceGroupController> logger)
        {
            _serviceGroupService = serviceGroupService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves service group data by ID.
        /// </summary>
        /// <param name="groupId">Service group ID.</param>
        /// <returns>
        /// Returns 200 OK with service group data if successful;
        /// 401 Unauthorized if the user is not authenticated;
        /// 404 Not Found if the service gorup does not exist;
        /// </returns>
        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetServiceGroupById(int groupId)
        {
            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Retrieving service group data: {ServiceGroupId}", groupId);
                var result = await _serviceGroupService.GetByIdAsync(groupId, businessId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to get service group: {GroupId}, reason: {Reason}", groupId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return Ok(result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during retrieval of service group data.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new service group for the current business.
        /// </summary>
        /// <param name="dto">The group creation data.</param>
        /// <returns>
        /// 201 Created with the created group;
        /// 400 Bad Request if validation fails;
        /// 401 Unauthorized if not authenticated.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> AddServiceGroup([FromBody] ServiceGroupCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in AddServiceGroup.");
                return BadRequest(ModelState);
            }

            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Adding new service group.");
                var result = await _serviceGroupService.AddServiceGroupAsync(businessId, dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to add service group: {Reason}", result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return CreatedAtAction(nameof(GetServiceGroupById), new { serviceGroupId = result.Data.Id }, result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during creating of service group.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a service group from the current business.
        /// </summary>
        /// <param name="groupId">The ID of the service group to delete.</param>
        /// <returns>
        /// 204 No Content if successful;
        /// 401 Unauthorized if not authenticated;
        /// 404 Not Found if the group does not exist or does not belong to the business;
        /// 405 if trying to delete a system group;
        /// </returns>
        [HttpDelete("{groupId}")]
        public async Task<IActionResult> DeleteServiceGroup(int groupId)
        {
            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Deleting service group: {GroupId}", groupId);
                var result = await _serviceGroupService.DeleteServiceGroupAsync(groupId, businessId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to delete service group: {GroupId}, reason: {Reason}", groupId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during deleting of service group.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the name of a service group.
        /// </summary>
        /// <param name="groupId">The ID of the group to update.</param>
        /// <param name="dto">The new group data.</param>
        /// <returns>
        /// 200 OK with updated group;
        /// 400 Bad Request if validation fails;
        /// 401 Unauthorized;
        /// 404 Not Found if the group does not exist or does not belong to the business.
        /// 405 if trying to update a system group.
        /// </returns>
        [HttpPatch("{groupId}")]
        public async Task<IActionResult> UpdateServiceGroup(int groupId, [FromBody] ServiceGroupUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in AddServiceGroup.");
                return BadRequest(ModelState);
            }

            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Updating service group data: {GroupId}", groupId);
                var result = await _serviceGroupService.UpdateServiceGroupAsync(groupId, businessId, dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to update service group data: {GroupId}, resaon: {Reason}", groupId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return Ok(result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt while updating a service group data.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the order of services within a group.
        /// </summary>
        /// <param name="groupId">The ID of the group to reorder.</param>
        /// <param name="dto">The reorder data.</param>
        /// <returns>
        /// 200 OK with the updated list of service groups;
        /// 400 Bad Request if the input is invalid;
        /// 401 Unauthorized;
        /// 404 Not Found if the group does not exist or does not belong to the business;
        /// 405 if the group is marked as system and can't be moved;
        /// </returns>
        [HttpPatch("{groupId}/order")]
        public async Task<IActionResult> UpdateServiceGroupOrder(int groupId, [FromBody] ServiceGroupReorderDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in AddServiceGroup.");
                return BadRequest(ModelState);
            }

            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Trying reorder group: {GroupId}", groupId);
                var result = await _serviceGroupService.ReorderServiceGroupAsync(groupId, businessId, dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to reorder service group: {GroupId}, resaon: {Reason}", groupId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return Ok(result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt while reordering a service groups.");
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
