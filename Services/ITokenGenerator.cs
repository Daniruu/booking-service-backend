using BookingService.Models;

namespace BookingService.Services
{
    public interface ITokenGenerator
    {
        string GenerateToken(Account account);
        Task<string> GenerateRefreshToken();
    }
}
