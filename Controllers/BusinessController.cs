using BookingService.DTOs;
using BookingService.Extensions;
using BookingService.Models;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    /// <summary>
    /// Controller responsible for managing business accounts. 
    /// Handles requests related to the currently authenticated business account as well as public business listings.
    /// </summary>
    [ApiController]
    [Route("api/businesses")]
    [Authorize]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessService _businessService;
        private readonly ILogger<BusinessController> _logger;

        public BusinessController(IBusinessService businessService, ILogger<BusinessController> logger)
        {
            _businessService = businessService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the profile information of the currently authenticated business account.
        /// </summary>
        /// <returns>
        /// Returns 200 OK with business profile data if successful;
        /// 401 Unauthorized if the business is not authenticated;
        /// 404 Not Found if the business does not exist;
        /// </returns>
        /// <response code="200">Returns the business profile</response>
        /// <response code="401">Business is not authenticated</response>
        /// <response code="404">Business not found</response>
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentBusiness()
        {
            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Retrieving current business info for business ID: {BusinessId}", businessId);
                var result = await _businessService.GetByIdAsync(businessId);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to get business: {BusinessId}, reason: {Reason}", businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                return Ok(new { businessDto = result.Data });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during retrieval of current business.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates basic profile information of the current business account,
        /// such as name, description, and category.
        /// </summary>
        /// <param name="dto">Data to update.</param>
        /// <returns>
        /// 204 No Content if update is successful;
        /// 400 Bad Request if the data is invalid or empty;
        /// 401 Unauthorized if user is not authenticated;
        /// 404 Not Found if business or category is missing.
        /// </returns>
        /// <response code="204">Profile updated successfully</response>
        /// <response code="400">Invalid data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Business or category not found</response>
        [HttpPatch("me")]
        public async Task<IActionResult> UpdateBusiness([FromBody] PatchBusinessDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in UpdateBusiness.");
                return BadRequest(ModelState);
            }

            if (dto.Name == null && dto.Description == null && dto.CategoryId == null)
            {
                _logger.LogWarning("Business update attempt with empty payload.");
                return BadRequest(new { message = "At least one field must be provided." });
            }

            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Updating basic info for business ID: {BusinessId}", businessId);
                var result = await _businessService.UpdateBusinessAsync(businessId, dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to update business: {BusinessId}, reason: {Reason}", businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Business {BusinessId} successfully updated their profile.", businessId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during update of current business data.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the registration data (NIP, REGON, KRS) of the currently authenticated business.
        /// </summary>
        /// <param name="dto">The registration data to update.</param>
        /// <returns>
        /// 204 No Content if successful;
        /// 400 Bad Request if no fields are provided or validation fails;
        /// 401 Unauthorized if not authenticated;
        /// 404 Not Found if the business is not found.
        /// </returns>
        /// <response code="204">Registration data updated successfully</response>
        /// <response code="400">Invalid input or empty payload</response>
        /// <response code="401">User not authenticated</response>
        /// <response code="404">Business not found</response>
        [HttpPatch("me/registration-data")]
        public async Task<IActionResult> UpdateBusinessRegistrationData([FromBody] PatchBusinessRegistrationDataDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in UpdateBusinessRegistrationData.");
                return BadRequest(ModelState);
            }

            if (dto.Nip == null && dto.Regon == null && dto.Krs == null)
            {
                _logger.LogWarning("Business registration data update attempt with empty payload.");
                return BadRequest(new { message = "At least one field must be provided." });
            }

            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Updating registration data for business ID: {BusinessId}", businessId);
                var result = await _businessService.UpdateRegistrationDataAsync(businessId, dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to update registration data for business {BusinessId}: {Reason}", businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Business {BusinessId} successfully updated registration data.", businessId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt during update of business registration data.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the address of the currently authenticated business account.
        /// </summary>
        /// <param name="dto">The address data to update.</param>
        /// <returns>
        /// 204 No Content if the update is successful;
        /// 400 Bad Request if validation fails;
        /// 401 Unauthorized if not authenticated;
        /// 404 Not Found if business is not found.
        /// </returns>
        /// <response code="204">Address updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Business not found</response>
        [HttpPatch("me/address")]
        public async Task<IActionResult> UpdateBusinessAddress([FromBody] UpdateAddressDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in UpdateBusinessAddress.");
                return BadRequest(ModelState);
            }

            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Updating address for business {BusinessId}.", businessId);
                var result = await _businessService.UpdateAddressAsync(businessId, dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to update address for business {BusinessId}: {Reason}", businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Business {BusinessId} successfully updated address.", businessId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access during address update.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates booking settings of the currently authenticated business,
        /// such as auto-confirmation and buffer time.
        /// </summary>
        /// <param name="dto">The settings data to update.</param>
        /// <returns>
        /// 204 No Content if successful;
        /// 400 Bad Request if validation fails or no fields provided;
        /// 401 Unauthorized if not authenticated;
        /// 404 Not Found if business does not exist.
        /// </returns>
        /// <response code="204">Settings updated successfully</response>
        /// <response code="400">Invalid input or empty payload</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Business not found</response>
        [HttpPatch("me/settings")]
        public async Task<IActionResult> UpdateBusinessSettings([FromBody] PatchBusinessSettingsDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in UpdateBusinessSettings.");
                return BadRequest(ModelState);
            }

            if (dto.AutoConfirmBookings == null && dto.BookingBufferTime == null)
            {
                _logger.LogWarning("Settings update attempt with empty payload.");
                return BadRequest(new { message = "At least one field must be provided." });
            }

            try
            {
                var businessId = User.GetUserId();

                _logger.LogInformation("Updating settings for business {BusinessId}.", businessId);
                var result = await _businessService.UpdateSettingsAsync(businessId, dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to update settings for business {BusinessId}: {Reason}", businessId, result.ErrorMessage);
                    return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
                }

                _logger.LogInformation("Settings updated successfully for business {BusinessId}.", businessId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access during update of business settings.");
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a paginated and optionally filtered list of published businesses.
        /// </summary>
        /// <param name="query">Filter and pagination parameters.</param>
        /// <returns>
        /// 200 OK with paginated business list;
        /// 400 Bad Request if query parameters are invalid.
        /// </returns>
        /// <response code="200">List of businesses returned successfully</response>
        /// <response code="400">Invalid query parameters</response>
        [HttpGet("list")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicBusinesses([FromQuery] BusinessQueryParams query)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state in GetPublicBusinesses.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Retrieving published businesses: Page {Page}, Category {CategoryId}, City {City}, Search '{Search}'",
                query.Page, query.CategoryId, query.City, query.SearchTerms);

            var result = await _businessService.GetPaginatedBusinessesAsync(query);
            if (!result.Success)
            {
                _logger.LogWarning("Failed to get businesses: {Reason}", result.ErrorMessage);
                return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
            }

            return Ok(new { businessListDto = result.Data });
        }

        /// <summary>
        /// Retrieves the public details of a specific business by its ID.
        /// </summary>
        /// <param name="businessId">The ID of the business.</param>
        /// <returns>
        /// 200 OK with business data if found;
        /// 404 Not Found if the business does not exist or is not published.
        /// </returns>
        /// <response code="200">Business details returned successfully</response>
        /// <response code="404">Business not found or not published</response>
        [HttpGet("{businessId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBusinessById(int businessId)
        {
            var userId = User.TryGetUserId(out var id) ? id : (int?)null;

            _logger.LogInformation("Fetching public business details for ID {BusinessId}. Authenticated: {Auth}", businessId, userId != null);

            var result = await _businessService.GetPublicBusinessByIdAsync(businessId, userId);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to load business {BusinessId}: {Reason}", id, result.ErrorMessage);
                return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
            }

            return Ok(new { businessPublicDetailsDto = result.Data });
        }
    }
}
