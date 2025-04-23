using BookingService.DTOs;
using BookingService.Utils;

namespace BookingService.Services
{
    /// <summary>
    /// Service responsible for managing both business and employee weekly schedules.
    /// </summary>
    public interface IScheduleService
    {
        /// <summary>
        /// Gets the current weekly schedule for the business.
        /// </summary>
        Task<ServiceResult<List<DayScheduleDto>>> GetBusinessScheduleAsync(int businessId);

        /// <summary>
        /// Replaces the entire weekly schedule for the business with the provided time slots.
        /// </summary>
        Task<ServiceResult> UpdateBusinessScheduleAsync(int businessId, List<DayScheduleUpdateDto> scheduleDto);

        /// <summary>
        /// Gets the current weekly schedule for the employee.
        /// </summary>
        Task<ServiceResult<List<DayScheduleDto>>> GetEmployeeScheduleAsync(int employeeId, int businessId);

        /// <summary>
        /// Replaces the entrire weekly schedule for employee with the provided time slots.
        /// </summary>
        Task<ServiceResult> UpdateEmployeeScheduleAsync(int employeeId, int businessId, List<DayScheduleUpdateDto> scheduleDto);
    }
}
