using BookingService.Models;
using BookingService.Specifications;

namespace BookingService.Repositories
{
    public interface IBusinessRepository
    {
        Task<Business?> GetByIdAsync(int id, BusinessSpecifications? spec = null);
        Task<Business?> GetByEmailAsync(string email);
        Task<Business?> GetByNipAsync(string nip);
        Task<Business?> GetByPhoneNumberAsync(string phoneNumber);
        Task UpdateAsync(Business business);
        Task<List<Business?>> GetPaginatedBusinessesAsync(int page, int limit, int? categoryId = null, string? location = null, string? searchTerms = null);
        Task<int> CountBusinessesAsync(int? categoryId = null, string? location = null, string? searchTerms = null);
    }
}
