using BookingService.Models;

namespace BookingService.Repositories
{
    public interface IConfirmationCodeRepository
    {
        Task AddAsync(ConfirmationCode code);
        Task Add(ConfirmationCode code);
        Task Update(ConfirmationCode code);
        Task<ConfirmationCode> GetByEmailAndTypeAsync(string email, ConfirmationCodeType type);
        Task Delete(ConfirmationCode code);
        Task<List<ConfirmationCode>> GetExpiredCodesAsync();
        Task DeleteMultiple(List<ConfirmationCode> codeList);
    }
}
