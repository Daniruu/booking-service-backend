using BookingService.DTOs;
using BookingService.Utils;

namespace BookingService.Services
{
    public interface IAdminService
    {
        Task<ServiceResult<int>> AddBusinessCategoryAsync(BusinessCategoryCreateDto dto);
    }
}
