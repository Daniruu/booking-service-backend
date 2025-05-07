using BookingService.DTOs;
using BookingService.Utils;

namespace BookingService.Services
{
    public interface IServiceGroupService
    {
        /// <summary>
        /// Retrieves service groups belonging to the specified business.
        /// </summary>
        /// <param name="businessId">he ID of the business that owns the service groups.</param>
        /// <returns>Collection of service groups dtos or an error.</returns>
        Task<ServiceResult<IEnumerable<ServiceGroupDto>>> GetAllByBusinessIdAsync(int businessId);

        /// <summary>
        /// Retrieves a specific service group belonging to the specified business.
        /// </summary>
        /// <param name="groupId">The ID of the service group to retrieve.</param>
        /// <param name="businessId">The ID of the business that owns the service group.</param>
        /// <returns>
        /// A <see cref="ServiceResult{ServiceGroupDto}"/> containing the group if found;
        /// otherwise, an error with appropriate status code:
        /// 400 if input is invalid;
        /// 404 if business or group not found.
        /// </returns>
        Task<ServiceResult<ServiceGroupDto>> GetByIdAsync(int groupId, int businessId);

        /// <summary>
        /// Creates a new user-defined service group for the specified business.
        /// The group will be placed at the end of the current group list.
        /// </summary>
        /// <param name="businessId">The ID of the business for which the group is being created.</param>
        /// <param name="dto">The data used to create the new service group.</param>
        /// <returns>
        /// A <see cref="ServiceResult{ServiceGroupDto}"/> containing the created group;
        /// or an error if the business ID is invalid.
        /// </returns>
        Task<ServiceResult<ServiceGroupDto>> AddServiceGroupAsync(int businessId, CreateServiceGroupDto dto);

        /// <summary>
        /// Deletes a user-defined service group for the specified business.
        /// System-defined service groups cannot be deleted.
        /// After deletion, the remaining user groups are reordered.
        /// </summary>
        /// <param name="groupId">The ID of the group to delete.</param>
        /// <param name="businessId">The ID of the business that owns the group.</param>
        /// <returns>
        /// A <see cref="ServiceResult"/> indicating success;
        /// or an error with appropriate status code:
        /// 400 if IDs are invalid;
        /// 404 if business or group not found;
        /// 405 if trying to delete a system group.
        /// </returns>
        Task<ServiceResult> DeleteServiceGroupAsync(int groupId, int businessId);

        /// <summary>
        /// Updates the name or other editable fields of a user-defined service group.
        /// System groups cannot be updated.
        /// </summary>
        /// <param name="groupId">The ID of the service group to update.</param>
        /// <param name="businessId">The ID of the business that owns the group.</param>
        /// <param name="dto">The data used to update the group.</param>
        /// <returns>
        /// A <see cref="ServiceResult{ServiceGroupDto}"/> containing the updated group;
        /// or an error if validation fails:
        /// 400 if IDs are invalid;
        /// 404 if business or group not found;
        /// 405 if group is system-defined.
        /// </returns>
        Task<ServiceResult<ServiceGroupDto>> UpdateServiceGroupAsync(int groupId, int businessId, PatchServiceGroupDto dto);

        /// <summary>
        /// Reorders a service group within the business-defined service group list.
        /// System groups cannot be reordered, and user-defined groups are inserted only after them.
        /// </summary>
        /// <param name="groupId">The ID of the service group to move.</param>
        /// <param name="businessId">The ID of the business that owns the group.</param>
        /// <param name="dto">The new order position requested (1-based index).</param>
        /// <returns>
        /// A <see cref="ServiceResult{List{ServiceGroupDto}}"/> containing the updated, ordered list of service groups on success;
        /// otherwise, an error with appropriate HTTP status code:
        /// 400 if IDs are invalid;
        /// 404 if the business or group doesn't exist;
        /// 405 if the group is marked as system and can't be moved.
        /// </returns>
        Task<ServiceResult<IEnumerable<ServiceGroupDto>>> ReorderServiceGroupAsync(int groupId, int businessId, ReorderServiceGroupDto dto);
    }
}
