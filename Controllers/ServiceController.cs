using BookingService.DTOs;
using BookingService.Extensions;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    /// <summary>
    /// Handles service management for the current authenticated business,
    /// including creation, update, deletion, reordering and partial updates
    /// such as toggling the featured status.
    /// </summary>
    [ApiController]
    [Route("api/businesses/me/services")]
    [Authorize(Roles = "Business")]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly ILogger<ServiceController> _logger;

        public ServiceController(IServiceService serviceService, ILogger<ServiceController> logger)
        {
            _serviceService = serviceService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a service by its ID for the current authenticated business.
        /// </summary>
        /// <param name="serviceId">The ID of the service to retrieve.</param>
        /// <returns>
        /// 200 OK with the service data if found;
        /// 401 Unauthorized if the user is not authenticated;
        /// 404 Not Found if the service does not exist or does not belong to the current business.
        /// </returns>
        [HttpGet("{serviceId}")]
        public async Task<IActionResult> GetServiceById(int serviceId)
        {
            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Retrieving service data: {ServiceId}", serviceId);
                var result = await _serviceService.GetByIdAsync(serviceId, businessId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to get service: {ServiceId}, reason: {Reason}", serviceId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return Ok(result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during service retrieval.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Adds a new service to the current authenticated business account.
        /// </summary>
        /// <param name="dto">The data used to create the new service.</param>
        /// <returns>
        /// 201 Created with the created service DTO;
        /// 400 Bad Request if the model is invalid;
        /// 401 Unauthorized if the user is not authenticated;
        /// 500 Internal Server Error if creation fails.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> AddService([FromBody] ServiceCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in AddService.");
                return BadRequest(ModelState);
            }

            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Adding new service for business {BusinessId}.", businessId);
                var result = await _serviceService.AddServiceAsync(businessId, dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to add service for business {BusinessId}: {Reason}", businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Service {ServiceId} successfully created for business {BusinessId}.", result.Data.Id, businessId);
                return CreatedAtAction(nameof(GetServiceById), new { serviceId = result.Data.Id }, result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during service creation.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a service belonging to the current authenticated business.
        /// </summary>
        /// <param name="serviceId">The ID of the service to delete.</param>
        /// <returns>
        /// 204 No Content if deleted successfully;
        /// 401 Unauthorized if the user is not authenticated;
        /// 404 Not Found if the service does not exist or doesn't belong to the current business;
        /// 500 Internal Server Error if deletion fails.
        /// </returns>
        [HttpDelete("{serviceId}")]
        public async Task<IActionResult> DeleteService(int serviceId)
        {
            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Deleting service: {ServiceId} from business {BusinessId}.", serviceId, businessId);
                var result = await _serviceService.DeleteServiceAsync(serviceId, businessId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to delete service {ServiceId} for business {BusinessId}: {Reason}", serviceId, businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Service {ServiceId} successfully deleted.", serviceId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during service deletion.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the data of an existing service belonging to the current authenticated business.
        /// </summary>
        /// <param name="serviceId">The ID of the service to update.</param>
        /// <param name="dto">The updated service data.</param>
        /// <returns>
        /// 200 OK with the updated service;
        /// 400 Bad Request if the input data is invalid;
        /// 401 Unauthorized if the user is not authenticated;
        /// 404 Not Found if the service does not exist or does not belong to the business.
        /// </returns>
        [HttpPut("{serviceId}")]
        public async Task<IActionResult> UpdateService(int serviceId, [FromBody] ServiceUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in UpdateService.");
                return BadRequest(ModelState);
            }

            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Updating service {ServiceId} for business {BusinessId}.", serviceId, businessId);
                var result = await _serviceService.UpdateServiceAsync(serviceId, businessId, dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to update service {ServiceId} for business {BusinessId}: {Reason}", serviceId, businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Service {ServiceId} successfully updated.", serviceId);
                return Ok(result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt while updating service {ServiceId}.", serviceId);
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Changes the display order of the specified service within its group.
        /// </summary>
        /// <param name="serviceId">The ID of the service to reorder.</param>
        /// <param name="dto">The reorder request containing the new position.</param>
        /// <returns>
        /// 200 OK with the updated list of services;
        /// 400 Bad Request if input is invalid;
        /// 401 Unauthorized if the user is not authenticated;
        /// 404 Not Found if the service does not exist or does not belong to the business.
        /// </returns>
        [HttpPatch("{serviceId}/order")]
        public async Task<IActionResult> ReorderService(int serviceId, [FromBody] ServiceReorderDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in ReorderService.");
                return BadRequest(ModelState);
            }

            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Attempting to reorder service {ServiceId} in business {BusinessId}.", serviceId, businessId);
                var result = await _serviceService.ReorderServiceAsync(serviceId, businessId, dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to reorder service {ServiceId} in business {BusinessId}: {Reason}", serviceId, businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Service {ServiceId} successfully reordered.", serviceId);
                return Ok(result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt while reordering service {ServiceId}.", serviceId);
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates one or more boolean fields of a service, such as the featured flag.
        /// </summary>
        /// <param name="serviceId">The ID of the service to update.</param>
        /// <param name="dto">The fields to update (only boolean flags supported).</param>
        /// <returns>
        /// 200 OK with updated field(s);
        /// 400 Bad Request if input is invalid or no valid fields provided;
        /// 401 Unauthorized if the user is not authenticated;
        /// 404 Not Found if the service does not exist or does not belong to the business.
        /// </returns>
        [HttpPatch("{serviceId}")]
        public async Task<IActionResult> PatchService(int serviceId, [FromBody] ServicePatchDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in PatchService.");
                return BadRequest(ModelState);
            }

            if (dto.IsFeatured == null)
            {
                _logger.LogWarning("No valid fields provided to update in PatchService.");
                return BadRequest(new { message = "At least one field must be provided to update." });
            }

            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Patching service {ServiceId} for business {BusinessId}. Changes: {Changes}", serviceId, businessId, new { dto.IsFeatured });
                var result = await _serviceService.PatchServiceAsync(serviceId, businessId, dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to patch service {ServiceId}: {Reason}", serviceId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Service {ServiceId} updated. New values: {Updated}", serviceId, result.Data);
                return Ok(result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt while patching service {ServiceId}.", serviceId);
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
