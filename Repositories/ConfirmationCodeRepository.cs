using BookingService.Data;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories
{
    public class ConfirmationCodeRepository : IConfirmationCodeRepository
    {
        private readonly BookingServiceDbContext _context;
        public ConfirmationCodeRepository(BookingServiceDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ConfirmationCode code)
        {
            await _context.ConfirmationCodes.AddAsync(code);
            await _context.SaveChangesAsync();
        }

        public async Task Add(ConfirmationCode code)
        {
            _context.ConfirmationCodes.Add(code);
        }

        public async Task Update(ConfirmationCode code)
        {
            _context.ConfirmationCodes.Update(code);
        }
        public async Task<ConfirmationCode> GetByEmailAndTypeAsync(string email, ConfirmationCodeType type)
        {
            return await _context.ConfirmationCodes.Where(c => c.CodeType == type).FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task Delete(ConfirmationCode code)
        {
            _context.ConfirmationCodes.Remove(code);
        }

        public async Task<List<ConfirmationCode>> GetExpiredCodesAsync()
        {
            return await _context.ConfirmationCodes.Where(c => c.ExpirationTime < DateTime.Now).ToListAsync();
        }

        public async Task DeleteMultiple(List<ConfirmationCode> codeList)
        {
            _context.ConfirmationCodes.RemoveRange(codeList);
        }
    }
}
