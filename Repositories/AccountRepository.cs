using BookingService.Data;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BookingServiceDbContext _context;

        public AccountRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Set<Account>().AnyAsync(u => u.Email == email);
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            return await _context.Set<Account>().FindAsync(id);
        }

        public async Task<Account?> GetByEmailAsync(string email)
        {
            return await _context.Set<Account>().FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Account?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Set<Account>().FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }
        public async Task AddAsync(Account account)
        {
            _context.Set<Account>().Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Account account)
        {
            _context.Set<Account>().Update(account);
        }
    }
}
