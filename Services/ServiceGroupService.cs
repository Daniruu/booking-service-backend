using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Specifications;
using BookingService.Utils;

namespace BookingService.Services
{
    public class ServiceGroupService : IServiceGroupService
    {
        private readonly IServiceGroupRepository _serviceGroupRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ServiceGroupService> _logger;

        public ServiceGroupService(IServiceGroupRepository serviceGroupRepository, IBusinessRepository businessRepository, IMapper mapper, ILogger<ServiceGroupService> logger)
        {
            _serviceGroupRepository = serviceGroupRepository;
            _businessRepository = businessRepository;
            _mapper = mapper;
            _logger = logger;
        }

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
        public async Task<ServiceResult<ServiceGroupDto>> GetByIdAsync(int groupId, int businessId)
        {
            if (groupId < 0 ||  businessId < 0)
            {
                _logger.LogWarning("Invalid business or service group ID. BusinessId: {BusinessId}, GroupId: {GroupId}", businessId, groupId);
                return ServiceResult<ServiceGroupDto>.Failure("Invalid service group or business ID.", 400);
            }

            var business = await _businessRepository.GetByIdAsync(businessId, new BusinessSpecifications
            {
                IncludeServices = true,
            });

            if (business == null)
            {
                _logger.LogWarning("Business {BusinessId} not found while retrieving service group.", businessId);
                return ServiceResult<ServiceGroupDto>.Failure("Business not found.", 404);
            }

            var serviceGroup = business.ServiceGroups.FirstOrDefault(sg => sg.Id == groupId);
            if (serviceGroup == null)
            {
                _logger.LogWarning("Service group {GroupId} not found in business {BusinessId}.", groupId, businessId);
                return ServiceResult<ServiceGroupDto>.Failure("Service group not found.", 404);
            }

            _logger.LogInformation("Service group {GroupId} successfully retrieved.", groupId);
            var serviceGroupDto = _mapper.Map<ServiceGroupDto>(serviceGroup);
            return ServiceResult<ServiceGroupDto>.SuccessResult(serviceGroupDto);
        }

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
        public async Task<ServiceResult<ServiceGroupDto>> AddServiceGroupAsync(int businessId, ServiceGroupCreateDto dto)
        {
            if (businessId < 0)
            {
                _logger.LogWarning("Invalid business ID received: {BusinessId}", businessId);
                return ServiceResult<ServiceGroupDto>.Failure("Invalid business ID.", 400);
            }

            var existingGroups = await _serviceGroupRepository.GetAllByBusinessIdAsync(businessId);
            var maxOrder = existingGroups.Any() ? existingGroups.Max(g => g.Order) : 0;

            var serviceGroup = _mapper.Map<ServiceGroup>(dto);
            serviceGroup.BusinessId = businessId;
            serviceGroup.Order = maxOrder + 1;
            serviceGroup.IsSystem = false;

            await _serviceGroupRepository.AddAsync(serviceGroup);

            var serviceGroupDto = _mapper.Map<ServiceGroupDto>(serviceGroup);

            _logger.LogInformation("Service group {GroupId} created successfully in business {BusinessId}.", serviceGroup.Id, businessId);
            return ServiceResult<ServiceGroupDto>.SuccessResult(serviceGroupDto);
        }

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
        public async Task<ServiceResult> DeleteServiceGroupAsync(int groupId, int businessId)
        {
            if (groupId < 0 || businessId < 0)
            {
                _logger.LogWarning("Invalid service group or business ID received. GroupId: {GroupId}, BusinessId: {BusinessId}", groupId, businessId);
                return ServiceResult.Failure("Invalid service group or business ID.", 400);
            }

            var business = await _businessRepository.GetByIdAsync(businessId, new BusinessSpecifications
            {
                IncludeServices = true,
            });

            if (business == null)
            {
                _logger.LogWarning("Business {BusinessId} not found while attempting to delete service group.", businessId);
                return ServiceResult.Failure("Business not found.", 404);
            }

            var serviceGroup = business.ServiceGroups.FirstOrDefault(sg => sg.Id == groupId);
            if (serviceGroup == null)
            {
                _logger.LogWarning("Service group {GroupId} not found in business {BusinessId}.", groupId, businessId);
                return ServiceResult.Failure("Service group not found.", 404);
            }

            if (serviceGroup.IsSystem)
            {
                _logger.LogWarning("Attempt to delete system service group {GroupId} was blocked.", groupId);
                return ServiceResult.Failure("System service groups cannot be deleted.", 405);
            }

            await _serviceGroupRepository.DeleteAsync(serviceGroup);

            var remainingGroups = await _serviceGroupRepository.GetAllByBusinessIdAsync(businessId);
            var sortedGroups = remainingGroups.OrderBy(sg => sg.Order).ToList();

            for (int i = 0; i < sortedGroups.Count; i++)
                sortedGroups[i].Order = i + 1;

            await _serviceGroupRepository.UpdateRangeAsync(sortedGroups);

            _logger.LogInformation("Service group {GroupId} successfully deleted from business {BusinessId}.", groupId, businessId);
            return ServiceResult.SuccessResult();
        }

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
        public async Task<ServiceResult<ServiceGroupDto>> UpdateServiceGroupAsync(int groupId, int businessId, ServiceGroupUpdateDto dto)
        {
            if (groupId < 0 || businessId < 0)
            {
                _logger.LogWarning("Invalid business or service group ID. GroupId: {GroupId}, BusinessId: {BusinessId}", groupId, businessId);
                return ServiceResult<ServiceGroupDto>.Failure("Invalid service group or business ID.", 400);
            }

            var business = await _businessRepository.GetByIdAsync(businessId, new BusinessSpecifications
            {
                IncludeServices = true,
            });

            if (business == null)
            {
                _logger.LogWarning("Business {BusinessId} not found while updating service group {GroupId}.", businessId, groupId);
                return ServiceResult<ServiceGroupDto>.Failure("Business not found.", 404);
            }

            var serviceGroup = business.ServiceGroups.FirstOrDefault(sg => sg.Id == groupId);
            if (serviceGroup == null)
            {
                _logger.LogWarning("Service group {GroupId} not found in business {BusinessId}.", groupId, businessId);
                return ServiceResult<ServiceGroupDto>.Failure("Service group not found.", 404);
            }

            if (serviceGroup.IsSystem)
            {
                _logger.LogWarning("Attempt to update system service group {GroupId} was blocked.", groupId);
                return ServiceResult<ServiceGroupDto>.Failure("System service groups cannot be updated.", 405);
            }

            _mapper.Map(dto, serviceGroup);
            await _serviceGroupRepository.UpdateAsync(serviceGroup);

            var serviceGroupDto = _mapper.Map<ServiceGroupDto>(serviceGroup);

            _logger.LogInformation("Service group {GroupId} in business {BusinessId} updated successfully.", groupId, businessId);
            return ServiceResult<ServiceGroupDto>.SuccessResult(serviceGroupDto);
        }

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
        public async Task<ServiceResult<List<ServiceGroupDto>>> ReorderServiceGroupAsync(int groupId, int businessId, ServiceGroupReorderDto dto)
        {
            if (groupId < 0 || businessId < 0)
            {
                _logger.LogWarning("Invalid service group or business ID received. ServiceGroupId: {GroupId} BusinessId: {BusinessId}", groupId, businessId);
                return ServiceResult<List<ServiceGroupDto>>.Failure("Invalid service group or business ID.", 400);
            }

            var business = await _businessRepository.GetByIdAsync(businessId, new BusinessSpecifications
            {
                IncludeServices = true,
            });

            if (business == null)
            {
                _logger.LogWarning("Business {BusinessId} not found while updating service group order.", businessId);
                return ServiceResult<List<ServiceGroupDto>>.Failure("Business not found.", 404);
            }

            if (business.ServiceGroups == null || !business.ServiceGroups.Any())
            {
                _logger.LogWarning("Business {BusinessId} has no service groups while attempting to reorder group {GroupId}.", businessId, groupId);
                return ServiceResult<List<ServiceGroupDto>>.Failure("No service groups found for this business.", 404);
            }

            var groupToMove = business.ServiceGroups.FirstOrDefault(sg => sg.Id == groupId);
            if (groupToMove == null)
            {
                _logger.LogWarning("Service group {GroupId} not found while updating service group order.", groupId);
                return ServiceResult<List<ServiceGroupDto>>.Failure("Service group not found.", 404);
            }

            if (groupToMove.IsSystem)
            {
                _logger.LogWarning("Failed to reorder service group {GroupId}, system service groups cannot be updated.", groupId);
                return ServiceResult<List<ServiceGroupDto>>.Failure("System service groups cannot be updated.", 405);
            }

            var systemGroups = business.ServiceGroups.Where(sg => sg.IsSystem).OrderBy(sg => sg.Order).ToList();
            var userGroups = business.ServiceGroups.Where(sg => !sg.IsSystem).OrderBy(sg => sg.Order).ToList();

            int newIndex = dto.NewOrder - systemGroups.Count - 1;

            if (newIndex < 0)
            {
                _logger.LogInformation("Requested position is before system groups. Placing group immediately after system groups.");
                newIndex = 0;
            }

            if (newIndex >= userGroups.Count)
            {
                _logger.LogInformation("Requested position {NewOrder} exceeds number of user groups. Placing at end.", dto.NewOrder);
                newIndex = userGroups.Count - 1;
            }

            userGroups.Remove(groupToMove);
            userGroups.Insert(newIndex, groupToMove);

            for (int i = 0; i < userGroups.Count; i++)
            {
                userGroups[i].Order = systemGroups.Count + i + 1;
            }

            await _serviceGroupRepository.UpdateRangeAsync(userGroups);

            var allGroupsSorted = business.ServiceGroups.OrderBy(sg => sg.Order).ToList();
            allGroupsSorted.ForEach(g => g.Services = g.Services.OrderBy(s => s.Order).ToList());

            var updatedServiceGroupsDto = _mapper.Map<List<ServiceGroupDto>>(allGroupsSorted);

            _logger.LogInformation("Service group {GroupId} reordered successfully.", groupId);
            return ServiceResult<List<ServiceGroupDto>>.SuccessResult(updatedServiceGroupsDto);
        }
    }
}
