using BookingService.DTOs;
using BookingService.Utils;

namespace BookingService.Services
{
    public interface ICategoryService
    {
        Task<ServiceResult<int>> AddBusinessCategoryAsync(BusinessCategoryCreateDto dto);
        Task<ServiceResult<List<BusinessCategoryDto>>> GetAllCategories();
    }
}
