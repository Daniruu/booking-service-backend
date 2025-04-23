using BookingService.Models;
using BookingService.Specifications;

namespace BookingService.Repositories
{
    public interface IEmployeeRepository
    {
        Task AddAsync(Employee employee);
        Task<Employee> GetByIdAsync(int id, EmployeeSpecifications? spec = null);
        Task DeleteAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task<bool> EmployeeExistsAsync(string email);
    }
}
