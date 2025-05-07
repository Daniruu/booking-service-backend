using BookingService.DTOs;
using BookingService.Models;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    /// <summary>
    /// Handles registration-related operations such as user and business registration,
    /// and sending email confirmation codes.
    /// </summary>
    [ApiController]
    [Route("api/registrations")]
    [AllowAnonymous]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly IConfirmationCodeService _confirmationCodeService;
        private readonly ILogger<RegistrationController> _logger;
        public RegistrationController(
            IRegistrationService registrationService, 
            IConfirmationCodeService confirmationCodeService, 
            ILogger<RegistrationController> logger)
        {
            _registrationService = registrationService;
            _confirmationCodeService = confirmationCodeService;
            _logger = logger;
        }

        /// <summary>
        /// Sends an email confirmation code to the specified email address to verify user or business registration.
        /// </summary>
        /// <param name="dto">Email address to which the confirmation code should be sent.</param>
        /// <returns>Expiration time of the generated code or an error message.</returns>
        [HttpPost("email-code")]
        public async Task<IActionResult> RequestEmailCode(RequestEmailCodeDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for confirmation code request.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Requesting confirmation code for email: {Email}", dto.Email);
            var result = await _confirmationCodeService.GenerateAndSendAsync(dto.Email, ConfirmationCodeType.EmailConfirmaiton);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to send confirmation code to: {Email}, reason: {Reason}", dto.Email, result.ErrorMessage);
                return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
            }

            _logger.LogInformation("Confirmation code sent successfully to: {Email}", dto.Email);
            return Ok(new { expiresAt = result.Data });
        }

        [HttpPost("email-code/verifications")]
        public async Task<IActionResult> VerifyEmailCode(EmailCodeVerificationDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for verification code request.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Verifying confirmation code for email: {Email}", dto.Email);
            var result = await _confirmationCodeService.VerifyAsync(dto.Email, dto.Code, ConfirmationCodeType.EmailConfirmaiton);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to verify confirmation code to: {Email}, reason: {Reason}", dto.Email, result.ErrorMessage);
                return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
            }

            return NoContent();
        }

        /// <summary>
        /// Register a new user account.
        /// </summary>
        /// <param name="dto">User registration data including.</param>
        /// <returns>201 Created with the new user's ID or appropriate error message</returns>
        [HttpPost("users")]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user registration.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Registering new user: {Email}", dto.Email);
            var result = await _registrationService.RegisterUserAsync(dto);

            if (!result.Success)
            {
                _logger.LogWarning("User registration failed: {Email}, reason: {Reason}", dto.Email, result.ErrorMessage);
                return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
            }

            _logger.LogInformation("User registered successfully: {Email}, UserId: {Id}", dto.Email, result.Data);
            return Created($"/api/users/{result.Data}", new { userId = result.Data });
        }

        /// <summary>
        /// Registers a new business account.
        /// </summary>
        /// <param name="dto">Business registration data.</param>
        /// <returns>201 Created with the new business's ID or appropriate error message.</returns>
        [HttpPost("businesses")]
        public async Task<IActionResult> RegisterBusiness([FromBody] CreateBusinessDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for business registration.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Registering new business: {Email}", dto.Email);
            var result = await _registrationService.RegisterBusinessAsync(dto);

            if (!result.Success)
            {
                _logger.LogWarning("Business registration failed: {Email}, reason: {Reason}", dto.Email, result.ErrorMessage);
                return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
            }

            _logger.LogInformation("Business registered successfully: {Email}, BusinessId: {Id}", dto.Email, result.Data);
            return Created($"/api/businesses/{result.Data}", new { businessId = result.Data });
        }
    }
}
