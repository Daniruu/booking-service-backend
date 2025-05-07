using AutoMapper;
using BookingService.Data;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Utils;

namespace BookingService.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly BookingServiceDbContext _context;
        private readonly IBusinessCategoryRespository _businessCategoryRepository;
        private readonly IGoogleCloudStorageService _googleCloudStorageService;
        private readonly IMapper _mapper;

        public CategoryService(BookingServiceDbContext context, IBusinessCategoryRespository businessCategoryRepository, IGoogleCloudStorageService googleCloudStorageService, IMapper mapper)
        {
            _context = context;
            _businessCategoryRepository = businessCategoryRepository;
            _googleCloudStorageService = googleCloudStorageService;
            _mapper = mapper;
        }
        /// <summary>
        /// Adds a new business category with an associated icon image.
        /// </summary>
        /// <param name="dto">The data for the new business category.</param>
        /// <returns>
        /// A <see cref="ServiceResult{int}"/> containing the ID of the newly created category;
        /// or an error with appropriate status code if the operation fails.
        /// </returns>
        public async Task<ServiceResult<int>> AddBusinessCategoryAsync(CreateBusinessCategoryDto dto)
        {
            try
            {
                var existing = await _businessCategoryRepository.GetByNameAsync(dto.Name);
                if (existing != null)
                {
                    return ServiceResult<int>.Failure("Category with this name already exists.", 409);
                }

                string fileUrl;
                try
                {
                    fileUrl = await _googleCloudStorageService.UploadFileAsync("category_icons", dto.Icon);
                }
                catch (Exception ex)
                {
                    return ServiceResult<int>.Failure($"Error while uploading icon: {ex.Message}", 500);
                }

                var category = _mapper.Map<BusinessCategory>(dto);
                category.IconUrl = fileUrl;

                await _businessCategoryRepository.AddAsync(category);
                await _context.SaveChangesAsync();

                return ServiceResult<int>.SuccessResult(category.Id);
            }
            catch (Exception ex)
            {
                return ServiceResult<int>.Failure("An unexpected error occurred while adding the category.", 500);
            }
        }

        public async Task<ServiceResult<List<BusinessCategoryDto>>> GetAllCategories()
        {
            var categories = await _businessCategoryRepository.GetAll();
            var categoryDtos = _mapper.Map<List<BusinessCategoryDto>>(categories);

            return ServiceResult<List<BusinessCategoryDto>>.SuccessResult(categoryDtos);
        }
    }
}
