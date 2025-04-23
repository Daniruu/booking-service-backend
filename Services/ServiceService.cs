using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Specifications;
using BookingService.Utils;

namespace BookingService.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceGroupRepository _serviceGroupRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ServiceService> _logger;

        public ServiceService(
            IServiceRepository serviceRepository, 
            IServiceGroupRepository serviceGroupRepository,
            IBusinessRepository businessRepository,
            IMapper mapper,
            ILogger<ServiceService> logger
            )
        {
            _serviceRepository = serviceRepository;
            _serviceGroupRepository = serviceGroupRepository;
            _businessRepository = businessRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a service by its ID, ensuring it belongs to the specified business.
        /// </summary>
        /// <param name="serviceId">The ID of the service to retrieve.</param>
        /// <param name="businessId">The ID of the current authenticated business.</param>
        /// <returns>
        /// A <see cref="ServiceResult{ServiceDto}"/> containing the service if found;
        /// or 404/403 if the service does not exist or belongs to another business.
        /// </returns>
        public async Task<ServiceResult<ServiceDto>> GetByIdAsync(int serviceId, int businessId)
        {
            if (serviceId < 0 || businessId < 0)
            {
                _logger.LogWarning("Invalid IDs received: ServiceId={ServiceId}, BusinessId={BusinessId}", serviceId, businessId);
                return ServiceResult<ServiceDto>.Failure("Invalid identifiers.", 400);
            }

            var service = await _serviceRepository.GetByIdAsync(serviceId, new ServiceSpecifications
            {
                IncludeBusiness = true,
                IncludeEmployee = true,
            });

            if (service == null)
            {
                _logger.LogWarning("Service {ServiceId} not found.", serviceId);
                return ServiceResult<ServiceDto>.Failure("Service not found.", 404);
            }

            if (service.BusinessId != businessId)
            {
                _logger.LogWarning("Unauthorized access to service {ServiceId} by business {BusinessId}.", serviceId, businessId);
                return ServiceResult<ServiceDto>.Failure("Access denied.", 403);
            }

            var serviceDto = _mapper.Map<ServiceDto>(service);
            return ServiceResult<ServiceDto>.SuccessResult(serviceDto);
        }

        /// <summary>
        /// Creates a new service and assigns it to the specified business.
        /// </summary>
        /// <param name="businessId">The ID of the business to which the service should belong.</param>
        /// <param name="dto">The service creation data.</param>
        /// <returns>
        /// A <see cref="ServiceResult{ServiceDto}"/> with the newly created service;
        /// or 404 if the business does not exist.
        /// </returns>
        public async Task<ServiceResult<ServiceDto>> AddServiceAsync(int businessId, ServiceCreateDto dto)
        {
            if (businessId < 0)
            {
                _logger.LogWarning("Invalid business ID received: {BusinessId}", businessId);
                return ServiceResult<ServiceDto>.Failure("Invalid business ID.", 400);
            }

            var business = await _businessRepository.GetByIdAsync(businessId, new BusinessSpecifications
            {
                IncludeServices = true
            });

            if (business == null)
            {
                _logger.LogWarning("Business {BusinessId} not found while adding service.", businessId);
                return ServiceResult<ServiceDto>.Failure("Business not found.", 404);
            }

            var selectedGroup = business.ServiceGroups.FirstOrDefault(sg => sg.Id == dto.ServiceGroupId);
            if (selectedGroup == null)
            {
                _logger.LogWarning("Service group {GroupId} not found in business {BusinessId}.", dto.ServiceGroupId, businessId);
                return ServiceResult<ServiceDto>.Failure("Invalid service group.", 400);
            }

            var maxOrder = selectedGroup.Services.Any() ? selectedGroup.Services.Max(s => s.Order) : 0;

            var service = _mapper.Map<Service>(dto);
            service.BusinessId = businessId;
            service.Order = maxOrder + 1;
            service.IsFeatured = false;

            await _serviceRepository.AddAsync(service);

            var serviceDto = _mapper.Map<ServiceDto>(service);

            _logger.LogInformation("Service {ServiceId} created for business {BusinessId}.", service.Id, businessId);
            return ServiceResult<ServiceDto>.SuccessResult(serviceDto);
        }

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
        public async Task<ServiceResult> DeleteServiceAsync(int serviceId, int businessId)
        {
            if (serviceId < 0 || businessId < 0)
            {
                _logger.LogWarning("Invalid IDs for deletion. ServiceId: {ServiceId}, BusinessId: {BusinessId}", serviceId, businessId);
                return ServiceResult.Failure("Invalid service or business ID.", 400);
            }

            var service = await _serviceRepository.GetByIdAsync(serviceId, new ServiceSpecifications
            {
                IncludeBusiness = true,
            });

            if (service == null)
            {
                _logger.LogWarning("Service {ServiceId} not found.", serviceId);
                return ServiceResult.Failure("Service not found.", 404);
            }

            if (service.BusinessId != businessId)
            {
                _logger.LogWarning("Unauthorized deletion attempt. ServiceId: {ServiceId}, BusinessId: {BusinessId}", serviceId, businessId);
                return ServiceResult.Failure("Access denied.", 403);
            }

            await _serviceRepository.DeleteAsync(service);

            var remainingServices = await _serviceRepository.GetAllByServiceGroupId(service.ServiceGroupId);
            var sorted = remainingServices.OrderBy(s => s.Order).ToList();

            for (int i = 0; i < sorted.Count; i++)
            {
                sorted[i].Order = i + 1;
            }

            await _serviceRepository.UpdateRangeAsync(sorted);

            _logger.LogInformation("Service {ServiceId} deleted successfully from business {BusinessId}.", serviceId, businessId);
            return ServiceResult.SuccessResult();
        }

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
        public async Task<ServiceResult<ServiceDto>> UpdateServiceAsync(int serviceId, int businessId, ServiceUpdateDto dto)
        {
            if (serviceId < 0 || businessId < 0)
            {
                _logger.LogWarning("Invalid service or business ID. ServiceId: {ServiceId}, BusinessId: {BusinessId}", serviceId, businessId);
                return ServiceResult<ServiceDto>.Failure("Invalid service or business ID.", 400);
            }

            var service = await _serviceRepository.GetByIdAsync(serviceId, new ServiceSpecifications
            {
                IncludeBusiness = true,
            });

            if (service == null)
            {
                _logger.LogWarning("Service {ServiceId} not found.", serviceId);
                return ServiceResult<ServiceDto>.Failure("Service not found.", 404);
            }

            if (service.BusinessId != businessId)
            {
                _logger.LogWarning("Unauthorized attempt to update service {ServiceId} by business {BusinessId}.", serviceId, businessId);
                return ServiceResult<ServiceDto>.Failure("Access denied.", 403);
            }

            _mapper.Map(dto, service);
            await _serviceRepository.UpdateAsync(service);

            var serviceDto = _mapper.Map<ServiceDto>(service);

            _logger.LogInformation("Service {ServiceId} updated successfully by business {BusinessId}.", serviceId, businessId);
            return ServiceResult<ServiceDto>.SuccessResult(serviceDto);
        }

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
        public async Task<ServiceResult<UpdatedServiceGroupDto>> ReorderServiceAsync(int serviceId, int businessId, ServiceReorderDto dto)
        {
            if (serviceId < 0 || businessId < 0 || dto.NewOrder <= 0)
            {
                _logger.LogWarning("Invalid input for reorder. ServiceId: {ServiceId}, BusinessId: {BusinessId}, NewOrder: {NewOrder}", serviceId, businessId, dto.NewOrder);
                return ServiceResult<UpdatedServiceGroupDto>.Failure("Invalid input.", 400);
            }

            var serviceToMove = await _serviceRepository.GetByIdAsync(serviceId);
            if (serviceToMove == null)
            {
                _logger.LogWarning("Service {ServiceId} not found.", serviceId);
                return ServiceResult<UpdatedServiceGroupDto>.Failure("Service not found.", 404);
            }

            if (serviceToMove.BusinessId != businessId)
            {
                _logger.LogWarning("Unauthorized reorder attempt. ServiceId: {ServiceId}, BusinessId: {BusinessId}", serviceId, businessId);
                return ServiceResult<UpdatedServiceGroupDto>.Failure("Access denied.", 403);
            }

            var targetGroup = await _serviceGroupRepository.GetByIdAsync(dto.ServiceGroupId);
            if (targetGroup == null || targetGroup.BusinessId != businessId)
            {
                _logger.LogWarning("Target service group {GroupId} is invalid or does not belong to business {BusinessId}.", dto.ServiceGroupId, businessId);
                return ServiceResult<UpdatedServiceGroupDto>.Failure("Invalid service group.", 400);
            }

            var currentGroupId = serviceToMove.ServiceGroupId;
            bool isGroupChanged = currentGroupId != dto.ServiceGroupId;

            var servicesInCurrentGroup = await _serviceRepository.GetAllByServiceGroupId(currentGroupId);
            var servicesInTargetGroup = isGroupChanged
                ? await _serviceRepository.GetAllByServiceGroupId(dto.ServiceGroupId)
                : servicesInCurrentGroup;

            // If moved to another group — change group ID and update both groups
            if (isGroupChanged)
            {
                serviceToMove.ServiceGroupId = dto.ServiceGroupId;
                servicesInCurrentGroup.Remove(serviceToMove);
                servicesInTargetGroup.Add(serviceToMove);
            }

            int newIndex = dto.NewOrder - 1;
            newIndex = Math.Clamp(newIndex, 0, Math.Max(0, servicesInTargetGroup.Count - 1));

            // Reorder in target group
            servicesInTargetGroup = servicesInTargetGroup.OrderBy(s => s.Order).ToList();
            servicesInTargetGroup.Remove(serviceToMove);
            servicesInTargetGroup.Insert(newIndex, serviceToMove);

            for (int i = 0; i < servicesInTargetGroup.Count; i++)
                servicesInTargetGroup[i].Order = i + 1;

            await _serviceRepository.UpdateRangeAsync(servicesInTargetGroup);

            if (isGroupChanged)
            {
                for (int i = 0; i < servicesInCurrentGroup.Count; i++)
                    servicesInCurrentGroup[i].Order = i + 1;

                await _serviceRepository.UpdateRangeAsync(servicesInCurrentGroup);
            }

            var dtoResult = new UpdatedServiceGroupDto
            {
                IsGroupChanged = isGroupChanged,
                OldGroupServices = _mapper.Map<List<ServiceDto>>(servicesInCurrentGroup),
                NewGroupServices = _mapper.Map<List<ServiceDto>>(servicesInTargetGroup)
            };

            _logger.LogInformation("Service {ServiceId} moved to group {NewGroupId} at position {NewOrder}.", serviceId, dto.ServiceGroupId, dto.NewOrder);
            return ServiceResult<UpdatedServiceGroupDto>.SuccessResult(dtoResult);
        }

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
        public async Task<ServiceResult<ServiceDto>> PatchServiceAsync(int serviceId, int businessId, ServicePatchDto dto)
        {
            if (serviceId < 0 || businessId < 0)
            {
                _logger.LogWarning("Invalid IDs for patch. ServiceId: {ServiceId}, BusinessId: {BusinessId}", serviceId, businessId);
                return ServiceResult<ServiceDto>.Failure("Invalid IDs.", 400);
            }

            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service == null)
            {
                _logger.LogWarning("Service {ServiceId} not found.", serviceId);
                return ServiceResult<ServiceDto>.Failure("Service not found.", 404);
            }

            if (service.BusinessId != businessId)
            {
                _logger.LogWarning("Unauthorized patch attempt for service {ServiceId} by business {BusinessId}.", serviceId, businessId);
                return ServiceResult<ServiceDto>.Failure("Access denied.", 403);
            }

            // Apply partial updates
            if (dto.IsFeatured != null)
                service.IsFeatured = dto.IsFeatured.Value;

            await _serviceRepository.UpdateAsync(service);

            var updatedDto = _mapper.Map<ServiceDto>(service);
            _logger.LogInformation("Service {ServiceId} partially updated by business {BusinessId}.", serviceId, businessId);

            return ServiceResult<ServiceDto>.SuccessResult(updatedDto);
        }

    }
}
