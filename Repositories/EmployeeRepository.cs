using BookingService.Data;
using BookingService.Models;
using BookingService.Specifications;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly BookingServiceDbContext _context;

        public EmployeeRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Employee>> GetAllByBusinessIdAsync(int businessId)
        {
            return await _context.Employees
                .Where(e => e.BusinessId == businessId)
                .ToListAsync();
        }

        public async Task<Employee> GetByIdAsync(int id, EmployeeSpecifications? spec = null)
        {
            IQueryable<Employee> query = _context.Employees;

            if (spec?.IncludeBusiness == true)
                query = query.Include(e => e.Business);

            if (spec?.IncludeSchedule == true)
                query = query.Include(e => e.Schedule).ThenInclude(s => s.TimeSlots);

            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task DeleteAsync(Employee employee)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmployeeExistsAsync(string email)
        {
            return await _context.Employees.AnyAsync(e => e.Email == email);
        }
    }
}
