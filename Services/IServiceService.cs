using BookingService.DTOs;
using BookingService.Utils;

namespace BookingService.Services
{

    public interface IServiceService
    {
        /// <summary>
        /// Service responsible for managing business services, including creation,
        /// update, deletion, partial updates, and order management within service groups.
        /// </summary>
        /// <remarks>
        /// All methods ensure that services are accessed and modified only
        /// by the business that owns them. Services can also be reordered within their group,
        /// or moved between groups, while preserving display order.
        /// </remarks>
        Task<ServiceResult<ServiceDto>> GetByIdAsync(int serviceId, int businessId);

        /// <summary>
        /// Creates a new service and assigns it to the specified business.
        /// </summary>
        /// <param name="businessId">The ID of the business to which the service should belong.</param>
        /// <param name="dto">The service creation data.</param>
        /// <returns>
        /// A <see cref="ServiceResult{ServiceDto}"/> with the newly created service;
        /// or 404 if the business does not exist.
        /// </returns>
        Task<ServiceResult<ServiceDto>> AddServiceAsync(int businessId, CreateServiceDto dto);

        /// <summary>
        /// Deletes a service that belongs to the specified business.
        /// After deletion, the remaining services in the same group are re-ordered.
        /// </summary>
        /// <param name="serviceId">The ID of the service to delete.</param>
        /// <param name="businessId">The ID of the business that owns the service.</param>
        /// <returns>
        /// A <see cref="ServiceResult"/> indicating success;
        /// 400 if input is invalid;
        /// 403 if service does not belong to the business;
        /// 404 if service is not found.
        /// </returns>
        Task<ServiceResult> DeleteServiceAsync(int serviceId, int businessId);

        /// <summary>
        /// Updates an existing service that belongs to the specified business.
        /// </summary>
        /// <param name="serviceId">The ID of the service to update.</param>
        /// <param name="businessId">The ID of the business that owns the service.</param>
        /// <param name="dto">The new data to apply to the service.</param>
        /// <returns>
        /// A <see cref="ServiceResult{ServiceDto}"/> containing the updated service;
        /// or 404/403 if the service is not found or doesn't belong to the business.
        /// </returns>
        Task<ServiceResult<ServiceDto>> UpdateServiceAsync(int serviceId, int businessId, UpdateServiceDto dto);

        /// <summary>
        /// Changes the order of a service within its group, or moves it to another group if needed.
        /// Both the old and new group service orders are updated accordingly.
        /// </summary>
        /// <param name="serviceId">The ID of the service to move.</param>
        /// <param name="businessId">The ID of the authenticated business performing the action.</param>
        /// <param name="dto">The reorder request containing the target group and position.</param>
        /// <returns>
        /// A <see cref="ServiceResult{UpdatedServiceGroupDto}"/> with updated order information;
        /// 400 if the group is invalid;
        /// 403 if the service does not belong to the business;
        /// 404 if the service is not found.
        /// </returns>
        Task<ServiceResult<ServiceGroupUpdatedDto>> ReorderServiceAsync(int serviceId, int businessId, ReorderServiceDto dto);

        /// <summary>
        /// Applies partial updates to a service, such as toggling the featured flag.
        /// </summary>
        /// <param name="serviceId">The ID of the service to patch.</param>
        /// <param name="businessId">The ID of the business that owns the service.</param>
        /// <param name="dto">The patch data (supports only boolean flags).</param>
        /// <returns>
        /// A <see cref="ServiceResult{ServiceDto}"/> containing the updated service;
        /// or 404/403 if the service is not found or does not belong to the business.
        /// </returns>
        Task<ServiceResult<ServiceDto>> PatchServiceAsync(int serviceId, int businessId, PatchServiceDto dto);
    }
}
