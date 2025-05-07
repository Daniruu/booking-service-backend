using BookingService.DTOs;
using BookingService.Utils;

namespace BookingService.Services
{
    /// <summary>
    /// Service responsible for managing employees within a business account,
    /// including creation, update, deletion, and avatar management.
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// Retrieve employee list by business ID
        /// </summary>
        /// <param name="businessId">The Id of the business.</param>
        /// <returns>A service result with collection of employee data or an error.</returns>
        Task<ServiceResult<IEnumerable<EmployeeDto>>> GetByBusinessIdAsync(int businessId);

        /// <summary>
        /// Retrieve an employee by ID.
        /// </summary>
        /// <param name="businessId">The ID of the current authenticated business.</param>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <returns>A service result with the employee data or an error.</returns>
        Task<ServiceResult<EmployeeDto>> GetByIdAsync(int employeeId, int businessId);

        /// <summary>
        /// Adds a new employee to the current business.
        /// </summary>
        /// <param name="businessId">The ID of the current authenticated business.</param>
        /// <param name="dto">The data for the new employee.</param>
        /// <returns>A service result with the created employee DTO or an error.</returns>
        Task<ServiceResult<EmployeeDto>> AddEmployeeAsync(int businessId, CreateEmployeeDto dto);

        /// <summary>
        /// Updates an existing employee.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <param name="businessId">The ID of the current authenticated business.</param>
        /// <param name="dto">Updated employee data.</param>
        /// <returns>A service result with the updated employee DTO or an error.</returns>
        Task<ServiceResult<EmployeeDto>> UpdateEmployeeAsync(int employeeId, int businessId, UpdateEmployeeDto dto);

        /// <summary>
        /// Deletes an employee by ID.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <param name="businessId">The ID of the current authenticated business.</param>
        /// <returns>A service result indicating success or failure.</returns>
        Task<ServiceResult> DeleteEmployeeAsync(int employeeId, int businessId);

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
        Task<ServiceResult<string>> UploadAvatarAsync(int employeeId, int businessId, IFormFile file);

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
        Task<ServiceResult> DeleteAvatarAsync(int employeeId, int businessId);
    }
}
