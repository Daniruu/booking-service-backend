using BookingService.DTOs;
using BookingService.Utils;

namespace BookingService.Services
{
    /// <summary>
    /// Handles the registration process for user and business accounts.
    /// Validates email, confirmation code, and business category (if applicable).
    /// </summary>
    public interface IRegistrationService
    {
        /// <summary>
        /// Registers a new user account after verifying email and confirmation code.
        /// </summary>
        /// <param name="dto">Data required to register a user, including email, password, phone number, and confirmation code.</param>
        /// <returns>ID of the newly created user or an error.</returns>
        Task<ServiceResult<int>> RegisterUserAsync(CreateUserDto dto);

        /// <summary>
        /// Registers a new business account after verifying email, confirmation code, and business category.
        /// </summary>
        /// <param name="dto">Data required to register a business, including category ID, email, password, and confirmation code.</param>
        /// <returns>ID of the newly created business or an error.</returns>
        Task<ServiceResult<int>> RegisterBusinessAsync(CreateBusinessDto dto);
    }
}
