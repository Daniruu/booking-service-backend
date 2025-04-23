using BookingService.Models;

namespace BookingService.Repositories
{
    public interface IScheduleRepository
    {
        Task<List<DaySchedule>> GetByBusinessIdAsync(int businessId);
        Task<List<DaySchedule>> GetByEmployeeIdAsync(int employeeId);
        Task DeleteByBusinessIdAsync(int businessId);
        Task DeleteByEmployeeIdAsync(int employeeId);
        Task AddRangeAsync(List<DaySchedule> schedules);
    }
}
