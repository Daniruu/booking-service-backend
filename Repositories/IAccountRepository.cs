using BookingService.Models;

namespace BookingService.Repositories
{
    public interface IAccountRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task<Account?> GetByIdAsync(int id);
        Task<Account?> GetByEmailAsync(string email);
        Task<Account?> GetUserByRefreshTokenAsync(string refreshToken);
        Task AddAsync(Account account);
        Task Update(Account account);
    }
}
