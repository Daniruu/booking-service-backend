using BookingService.DTOs;
using BookingService.Utils;
using System.Collections.Generic;

namespace BookingService.Services
{
    public interface IBusinessService
    {
        /// <summary>
        /// Retrieves a business by its ID along with related data (address, settings, images, schedule, employees, and services).
        /// </summary>
        /// <param name="businessId">The ID of the business to retrieve.</param>
        /// <returns>
        /// A <see cref="ServiceResult{BusinessDto}"/> containing the business data if found,
        /// or an error result with 404 Not Found if the business does not exist.
        /// </returns>
        Task<ServiceResult<BusinessDto>> GetByIdAsync(int businessId);

        /// <summary>
        /// Updates the specified business's name, description, and category ID.
        /// </summary>
        /// <param name="businessId">The ID of the business to update.</param>
        /// <param name="dto">DTO containing the fields to update.</param>
        /// <returns>
        /// A <see cref="ServiceResult"/> indicating success or failure.
        /// </returns>
        Task<ServiceResult> UpdateBusinessAsync(int businessId, PatchBusinessDto dto);

        /// <summary>
        /// Updates the registration data (NIP, REGON, KRS) for the specified business.
        /// </summary>
        /// <param name="businessId">The ID of the business to update.</param>
        /// <param name="dto">The data containing updated registration fields.</param>
        /// <returns>
        /// A <see cref="ServiceResult"/> indicating success or failure.
        /// </returns>
        Task<ServiceResult> UpdateRegistrationDataAsync(int businessId, PatchBusinessRegistrationDataDto dto);

        /// <summary>
        /// Updates the address for the specified business.
        /// </summary>
        /// <param name="businessId">The ID of the business whose address is being updated.</param>
        /// <param name="dto">The updated address data.</param>
        /// <returns>
        /// A <see cref="ServiceResult"/> indicating the outcome of the operation.
        /// </returns>
        Task<ServiceResult> UpdateAddressAsync(int businessId, UpdateAddressDto dto);

        /// <summary>
        /// Updates booking-related settings for the specified business,
        /// including auto-confirmation and buffer time between bookings.
        /// </summary>
        /// <param name="businessId">The ID of the business.</param>
        /// <param name="dto">Settings data to apply.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating success or failure.</returns>
        Task<ServiceResult> UpdateSettingsAsync(int businessId, PatchBusinessSettingsDto dto);

        /// <summary>
        /// Retrieves a paginated list of published businesses based on the provided filters and pagination settings.
        /// </summary>
        /// <param name="query">Filter and pagination parameters.</param>
        /// <returns>
        /// A <see cref="ServiceResult{BusinessListDto}"/> containing the list of businesses and pagination metadata.
        /// </returns>
        Task<ServiceResult<BusinessListDto>> GetPaginatedBusinessesAsync(BusinessQueryParams query);

        /// <summary>
        /// Retrieves a single published business by ID with full public details.
        /// </summary>
        /// <param name="businessId">The ID of the business to retrieve.</param>
        /// <returns>
        /// A <see cref="ServiceResult{BusinessDto}"/> containing the business if found;
        /// or 404 if not published or not found.
        /// </returns>
        Task<ServiceResult<BusinessPublicDetailsDto>> GetPublicBusinessByIdAsync(int businessId, int? userId);
    }
}
