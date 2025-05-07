using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Specifications;
using BookingService.Utils;

namespace BookingService.Services
{
    /// <summary>
    /// Service responsible for managing employees within a business account,
    /// including creation, update, deletion, and avatar management.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly IBusinessRepository _businessRepository;
        private readonly IGoogleCloudStorageService _googleCloudStorageService;
        private readonly ILogger<EmployeeService> _logger;
        public EmployeeService(
            IEmployeeRepository employeeRepository, 
            IMapper mapper,
            IBusinessRepository businessRepository, 
            IGoogleCloudStorageService googleCloudStorageService,
            ILogger<EmployeeService> logger
            )
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _businessRepository = businessRepository;
            _googleCloudStorageService = googleCloudStorageService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieve employee list by business ID
        /// </summary>
        /// <param name="businessId">The Id of the business.</param>
        /// <returns>A service result with collection of employee data or an error.</returns>
        public async Task<ServiceResult<IEnumerable<EmployeeDto>>> GetByBusinessIdAsync(int businessId)
        {
            if (businessId < 0)
            {
                _logger.LogWarning("Invalid business ID received. BusinessId: {BusinessId}", businessId);
                return ServiceResult<IEnumerable<EmployeeDto>>.Failure("Invalid business ID.", 400);
            }

            var employees = await _employeeRepository.GetAllByBusinessIdAsync(businessId);
            var employeeDtos = _mapper.Map<List<EmployeeDto>>(employees);

            _logger.LogInformation("Successfully retrieved {Count} employees for business {BusinessId}.", employeeDtos.Count, businessId);
            return ServiceResult<IEnumerable<EmployeeDto>>.SuccessResult(employeeDtos);
        }

        /// <summary>
        /// Retrieve an employee by ID.
        /// </summary>
        /// <param name="businessId">The ID of the current authenticated business.</param>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <returns>A service result with the employee data or an error.</returns>
        public async Task<ServiceResult<EmployeeDto>> GetByIdAsync(int employeeId, int businessId)
        {
            if (employeeId < 0 || businessId < 0)
            {
                _logger.LogWarning("Invalid employee or business ID received. EmployeeId: {EmployeeId} BusinessId: {BusinessId}", employeeId, businessId);
                return ServiceResult<EmployeeDto>.Failure("Invalid employee or business ID.", 400);
            }

            var business = await _businessRepository.GetByIdAsync(businessId, new BusinessSpecifications
            {
                IncludeEmployees = true,
            });

            if (business == null)
            {
                _logger.LogWarning("Business {BusinessId} not found while retrieving employee data.", businessId);
                return ServiceResult<EmployeeDto>.Failure("Business not found.", 404);
            }

            var employee = business.Employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee == null)
            {
                _logger.LogWarning("Employee {EmployeeId} not found while retrieving employee data.", employeeId);
                return ServiceResult<EmployeeDto>.Failure("Employee not found.", 404);
            }

            var employeeDto = _mapper.Map<EmployeeDto>(employee);
            return ServiceResult<EmployeeDto>.SuccessResult(employeeDto);
        }

        /// <summary>
        /// Adds a new employee to the current business.
        /// </summary>
        /// <param name="businessId">The ID of the current authenticated business.</param>
        /// <param name="dto">The data for the new employee.</param>
        /// <returns>A service result with the created employee DTO or an error.</returns>
        public async Task<ServiceResult<EmployeeDto>> AddEmployeeAsync(int businessId, CreateEmployeeDto dto)
        {
            if (businessId < 0)
            {
                _logger.LogWarning("Invalid business ID received: {BusinessId}", businessId);
                return ServiceResult<EmployeeDto>.Failure("Invalid business ID.", 400);
            }

            dto.Email = dto.Email.Trim().ToLower();
            dto.Phone = dto.Phone.Trim();

            var employee = _mapper.Map<Employee>(dto);
            employee.BusinessId = businessId;

            await _employeeRepository.AddAsync(employee);

            var employeeDto = _mapper.Map<EmployeeDto>(employee);

            _logger.LogInformation("Employee added successfully.");
            return ServiceResult<EmployeeDto>.SuccessResult(employeeDto);
        }

        /// <summary>
        /// Updates an existing employee.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <param name="businessId">The ID of the current authenticated business.</param>
        /// <param name="dto">Updated employee data.</param>
        /// <returns>A service result with the updated employee DTO or an error.</returns>
        public async Task<ServiceResult<EmployeeDto>> UpdateEmployeeAsync(int employeeId, int businessId, UpdateEmployeeDto dto)
        {
            if (employeeId < 0 || businessId < 0)
            {
                _logger.LogWarning("Invalid employee or business ID received. EmployeeId: {EmployeeId} BusinessId: {BusinessId}", employeeId, businessId);
                return ServiceResult<EmployeeDto>.Failure("Invalid employee or business ID.", 400);
            }

            dto.Email = dto.Email.Trim().ToLower();
            dto.Phone = dto.Phone.Trim();

            var business = await _businessRepository.GetByIdAsync(businessId, new BusinessSpecifications
            {
                IncludeEmployees = true,
            });

            if (business == null)
            {
                _logger.LogWarning("Business {BusinessId} not found while updating employee data.", businessId);
                return ServiceResult<EmployeeDto>.Failure("Business not found.", 404);
            }

            var employee = business.Employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee == null)
            {
                _logger.LogWarning("Employee {EmployeeId} not found while updating employee data.", employeeId);
                return ServiceResult<EmployeeDto>.Failure("Employee not found.", 404);
            }

            _mapper.Map(dto, employee);
            await _employeeRepository.UpdateAsync(employee);

            var employeeDto = _mapper.Map<EmployeeDto>(employee);

            _logger.LogInformation("Employee {EmployeeId} data successfully updated.", employeeId);
            return ServiceResult<EmployeeDto>.SuccessResult(employeeDto);
        }

        /// <summary>
        /// Deletes an employee by ID.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <param name="businessId">The ID of the current authenticated business.</param>
        /// <returns>A service result indicating success or failure.</returns>
        public async Task<ServiceResult> DeleteEmployeeAsync(int employeeId, int businessId)
        {
            if (employeeId < 0 || businessId < 0)
            {
                _logger.LogWarning("Invalid employee or business ID received. EmployeeId: {EmployeeId} BusinessId: {BusinessId}", employeeId, businessId);
                return ServiceResult<string>.Failure("Invalid employee or business ID.", 400);
            }

            var business = await _businessRepository.GetByIdAsync(businessId, new BusinessSpecifications
            {
                IncludeEmployees = true,
            });

            if (business == null)
            {
                _logger.LogWarning("Business {BusinessId} not found while deleting.", businessId);
                return ServiceResult<string>.Failure("Business not found.", 404);
            }

            var employee = business.Employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee == null)
            {
                _logger.LogWarning("Employee {EmployeeId} not found while deleting.", employeeId);
                return ServiceResult<string>.Failure("Employee not found.", 404);
            }

            await _employeeRepository.DeleteAsync(employee);
            return ServiceResult.SuccessResult();
        }

        /// <summary>
        /// Uploads a new avatar for the specified employee. If an existing avatar is present, it will be deleted first.
        /// </summary>
        /// <param name="employeeId">ID of the employee for whom the avatar is being uploaded.</param>
        /// <param name="businessId">The ID of the current authenticated business.</param>
        /// <param name="file">The image file to upload (must be jpg/jpeg/png, max 5MB).</param>
        /// <returns>
        /// A <see cref="ServiceResult{T}"/> containing the URL to the uploaded avatar on success,
        /// or an error message and status code on failure.
        /// </returns>
        public async Task<ServiceResult<string>> UploadAvatarAsync(int employeeId, int businessId, IFormFile file)
        {
            if (employeeId < 0 || businessId < 0)
            {
                _logger.LogWarning("Invalid employee or business ID received. EmployeeId: {EmployeeId} BusinessId: {BusinessId}", employeeId, businessId);
                return ServiceResult<string>.Failure("Invalid employee or business ID.", 400);
            }

            var business = await _businessRepository.GetByIdAsync(businessId, new BusinessSpecifications
            {
                IncludeEmployees = true,
            });

            if (business == null)
            {
                _logger.LogWarning("Business {BusinessId} not found while updating employee data.", businessId);
                return ServiceResult<string>.Failure("Business not found.", 404);
            }

            var employee = business.Employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee == null)
            {
                _logger.LogWarning("Employee {EmployeeId} not found while updating employee data.", employeeId);
                return ServiceResult<string>.Failure("Employee not found.", 404);
            }

            if (!string.IsNullOrEmpty(employee.AvatarUrl))
            {
                var existingFileName = employee.AvatarUrl.Split('/').Last();

                try
                {
                    await _googleCloudStorageService.DeleteFileAsync("employee-avatars", existingFileName);
                    _logger.LogInformation("Deleted previous avatar '{FileName}' for employee {EmployeeId}", existingFileName, employeeId);

                    employee.AvatarUrl = null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to delete previous avatar '{FileName}' for employee {EmployeeId}", existingFileName, employeeId);
                }
            }

            var fileUrl = await _googleCloudStorageService.UploadFileAsync("employee-avatars", file);
            _logger.LogInformation("Uploaded new avatar for employee {EmployeeId}: {Url}", employeeId, fileUrl);

            employee.AvatarUrl = fileUrl;
            await _employeeRepository.UpdateAsync(employee);

            return ServiceResult<string>.SuccessResult(fileUrl);
        }

        /// <summary>
        /// Deletes the employee's avatar from storage.
        /// </summary>
        /// <param name="employeeId">The ID of the employee whose avatar should be deleted.</param>
        /// <param name="businessId">The ID of the current authenticated business.</param>
        /// <returns>
        /// 204 No Content if successful;
        /// 404 Not Found if the employee or avatar does not exist;
        /// 401 Unauthorized if the user is not authenticated;
        /// 500 Internal Server Error if deletion fails.
        /// </returns>
        public async Task<ServiceResult> DeleteAvatarAsync(int employeeId, int businessId)
        {
            if (employeeId < 0 || businessId < 0)
            {
                _logger.LogWarning("Invalid employee or business ID received. EmployeeId: {EmployeeId} BusinessId: {BusinessId}", employeeId, businessId);
                return ServiceResult<string>.Failure("Invalid employee or business ID.", 400);
            }

            var business = await _businessRepository.GetByIdAsync(businessId, new BusinessSpecifications
            {
                IncludeEmployees = true,
            });

            if (business == null)
            {
                _logger.LogWarning("Business {BusinessId} not found while removing employee avatar.", businessId);
                return ServiceResult<string>.Failure("Business not found.", 404);
            }

            var employee = business.Employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee == null)
            {
                _logger.LogWarning("Employee {EmployeeId} not found while removing employee avatar.", employeeId);
                return ServiceResult<string>.Failure("Employee not found.", 404);
            }

            if (string.IsNullOrEmpty(employee.AvatarUrl))
            {
                _logger.LogWarning("No avatar to delete for employee {EmployeeId}", employeeId);
                return ServiceResult.Failure("No avatar to delete.", 404);
            }

            var fileName = employee.AvatarUrl.Split('/').Last();

            try
            {
                await _googleCloudStorageService.DeleteFileAsync("employee-avatars", fileName);
                _logger.LogInformation("Deleted avatar '{FileName}' for employee {EmployeeId}", fileName, employeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete avatar '{FileName}' for employee {EmployeeId}", fileName, employeeId);
                return ServiceResult.Failure("Failed to delete employee avatar from storage.", 500);
            }

            employee.AvatarUrl = null;
            await _employeeRepository.UpdateAsync(employee);

            return ServiceResult.SuccessResult();
        }
    }
}
