using BookingService.Data;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly BookingServiceDbContext _context;

        public ScheduleRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task<List<DaySchedule>> GetByBusinessIdAsync(int businessId)
        {
            return await _context.DaySchedules
                .Where(ds => ds.BusinessId == businessId)
                .Include(ds => ds.TimeSlots)
                .ToListAsync();
        }

        public async Task<List<DaySchedule>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.DaySchedules
                .Where(ds => ds.EmployeeId == employeeId)
                .Include(ds => ds.TimeSlots)
                .ToListAsync();
        }

        public async Task DeleteByBusinessIdAsync(int businessId)
        {
            var schedules = await _context.DaySchedules.Where(ds => ds.BusinessId == businessId).ToListAsync();
            _context.DaySchedules.RemoveRange(schedules);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByEmployeeIdAsync(int employeeId)
        {
            var schedules = await _context.DaySchedules.Where(ds => ds.EmployeeId == employeeId).ToListAsync();
            _context.DaySchedules.RemoveRange(schedules);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<DaySchedule> schedules)
        {
            await _context.DaySchedules.AddRangeAsync(schedules);
            await _context.SaveChangesAsync();
        }
    }
}
