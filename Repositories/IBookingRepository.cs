using BookingService.Data;
using BookingService.Models;
using BookingService.Specifications;
using System.Linq.Expressions;

namespace BookingService.Repositories
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(int id);
        Task<List<Booking>> GetAllByUserIdAsync(int userId);
        Task<List<Booking>> GetAllByBusinessIdAsync(int businessId);
        Task<List<Booking>> GetAllByEmployeeIdAsync(int employeeId, DateTimeOffset? date);
        Task AddRangeAsync(List<Booking> bookings);
        Task UpdateAsync(Booking booking);
        Task<bool> AnyAsync(Expression<Func<Booking, bool>> predicate);
    }
}
