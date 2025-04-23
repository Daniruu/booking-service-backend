using BookingService.Models;
using BookingService.Specifications;

namespace BookingService.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id, UserSpecifications? spec = null);
        Task<User?> GetByEmailAsync(string email);
        Task UpdateAsync(User user);
        Task<bool> EmailExistsAsync(string email);
    }
}
