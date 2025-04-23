using BookingService.Data;
using BookingService.Repositories;

namespace BookingService.Services
{
    public class CodeCleanupService : ICodeCleanupService
    {
        private readonly IConfirmationCodeRepository _codeRepository;
        private readonly BookingServiceDbContext _context;
        public CodeCleanupService(IConfirmationCodeRepository codeRepository, BookingServiceDbContext context)
        {
            _codeRepository = codeRepository;
            _context = context;
        }

        public async Task CleanupExpiredCodesAsync()
        {
            var expiredCodes = await _codeRepository.GetExpiredCodesAsync();

            if (expiredCodes.Any())
            {
                await _codeRepository.DeleteMultiple(expiredCodes);
                await _context.SaveChangesAsync();
            }
        }

    }
}
