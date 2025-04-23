using AutoMapper;
using BookingService.Data;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Specifications;
using BookingService.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Services
{
    public class BusinessService : IBusinessService
    {
        private readonly BookingServiceDbContext _context;
        private readonly IAuthService _authService;
        private readonly IBusinessRepository _businessRepository;
        private readonly IMapper _mapper;
        private readonly IBusinessCategoryRespository _categoryRepositopry;
        private readonly IBookingRepository _bookingRepository;
        private readonly IBookingService _bookingService;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IGoogleCloudStorageService _googleCloudStorageService;
        private readonly IBusinessImageRepository _businessImageRepository;
        private readonly IReviewRepository _reviewRepostiory;
        private readonly ILogger<BusinessService> _logger;

        public BusinessService(
            BookingServiceDbContext context,
            IAuthService authService,
            IBusinessRepository businessRepository,
            IMapper mapper,
            IBusinessCategoryRespository categoryRespository,
            IBookingRepository bookingRepository,
            IBookingService bookingService,
            IScheduleRepository scheduleRepository,
            IGoogleCloudStorageService googleCloudStorageService,
            IBusinessImageRepository businessImageRepository,
            IReviewRepository reviewRepository,
            ILogger<BusinessService> logger
            )
        {
            _context = context;
            _authService = authService;
            _businessRepository = businessRepository;
            _mapper = mapper;
            _categoryRepositopry = categoryRespository;
            _bookingRepository = bookingRepository;
            _bookingService = bookingService;
            _scheduleRepository = scheduleRepository;
            _googleCloudStorageService = googleCloudStorageService;
            _businessImageRepository = businessImageRepository;
            _reviewRepostiory = reviewRepository;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a business by its ID along with related data (address, settings, images, schedule, employees, and services).
        /// </summary>
        /// <param name="businessId">The ID of the business to retrieve.</param>
        /// <returns>
        /// A <see cref="ServiceResult{BusinessDto}"/> containing the business data if found,
        /// or an error result with 404 Not Found if the business does not exist.
        /// </returns>
        public async Task<ServiceResult<BusinessDto>> GetByIdAsync(int businessId)
        {
            if (businessId < 0)
            {
                _logger.LogWarning("Invalid user ID received: {BusinessId}", businessId);
                return ServiceResult<BusinessDto>.Failure("Invalid business ID.", 400);
            }

            var spec = new BusinessSpecifications
            {
                IncludeAddress = true,
                IncludeSettings = true,
                IncludeImages = true,
                IncludeSchedule = true,
                IncludeEmployees = true,
                IncludeServices = true,
            };

            var business = await _businessRepository.GetByIdAsync(businessId, spec);
            if (business == null)
            {
                _logger.LogWarning("Business with ID {BusinessId} not found.", businessId);
                return ServiceResult<BusinessDto>.Failure("Business not found.", 404);
            }

            _logger.LogInformation("Business with ID {BusinessId} retrieved successfully.", businessId);
            var businessDto = _mapper.Map<BusinessDto>(business);

            return ServiceResult<BusinessDto>.SuccessResult(businessDto);
        }

        /// <summary>
        /// Updates the specified business's name, description, and category ID.
        /// </summary>
        /// <param name="businessId">The ID of the business to update.</param>
        /// <param name="dto">DTO containing the fields to update.</param>
        /// <returns>
        /// A <see cref="ServiceResult"/> indicating success or failure.
        /// </returns>
        public async Task<ServiceResult> UpdateBusinessAsync(int businessId, BusinessUpdateDto dto)
        {

            if (businessId < 0)
            {
                _logger.LogWarning("Invalid business ID received: {BusinessId}", businessId);
                return ServiceResult.Failure("Invalid business ID.", 400);
            }

            var business = await _businessRepository.GetByIdAsync(businessId);
            if (business == null)
            {
                _logger.LogWarning("Business with ID {BusinessId} not found", businessId);
                return ServiceResult<UserDto>.Failure("Business not found.", 404);
            }

            if (dto.Name != null)
                business.Name = dto.Name;

            if (dto.Description != null)
                business.Description = dto.Description;

            if (dto.CategoryId != null)
            {
                if (await _categoryRepositopry.GetByIdAsync(dto.CategoryId.Value) == null)
                {
                    _logger.LogWarning("Invalid category ID {CategoryId} provided for business {BusinessId}", dto.CategoryId, businessId);
                    return ServiceResult.Failure("Invalid category.", 400);
                }

                business.CategoryId = dto.CategoryId.Value;
            }

            await _businessRepository.UpdateAsync(business);

            _logger.LogInformation("Business {BusinessId} updated. Fields changed: {Fields}", businessId, new
            {
                Name = dto.Name != null,
                Description = dto.Description != null,
                Category = dto.CategoryId != null
            });

            return ServiceResult.SuccessResult();
        }

        /// <summary>
        /// Updates the registration data (NIP, REGON, KRS) for the specified business.
        /// </summary>
        /// <param name="businessId">The ID of the business to update.</param>
        /// <param name="dto">The data containing updated registration fields.</param>
        /// <returns>
        /// A <see cref="ServiceResult"/> indicating success or failure.
        /// </returns>
        public async Task<ServiceResult> UpdateRegistrationDataAsync(int businessId, BusinessRegistrationDataUpdateDto dto)
        {
            var spec = new BusinessSpecifications
            {
                IncludeRegistration = true
            };

            var business = await _businessRepository.GetByIdAsync(businessId, spec);
            if (business == null)
            {
                _logger.LogWarning("Business with ID {BusinessId} not found during registration data update.", businessId);
                return ServiceResult.Failure("Business not found.", 404);
            }

            if (business.RegistrationData == null)
            {
                business.RegistrationData = new BusinessRegistrationData();
            }

            if (dto.Krs != null)
                business.RegistrationData.Krs = dto.Krs;

            if (dto.Nip != null)
                business.RegistrationData.Nip = dto.Nip;

            if (dto.Regon != null)
                business.RegistrationData.Regon = dto.Regon;

            await _businessRepository.UpdateAsync(business);

            _logger.LogInformation("Business {BusinessId} updated registration data. Fields changed: {Fields}", businessId, new
            {
                Krs = dto.Krs != null,
                Nip = dto.Nip != null,
                Regon = dto.Regon != null
            });

            return ServiceResult.SuccessResult();
        }

        /// <summary>
        /// Updates the address for the specified business.
        /// </summary>
        /// <param name="businessId">The ID of the business whose address is being updated.</param>
        /// <param name="dto">The updated address data.</param>
        /// <returns>
        /// A <see cref="ServiceResult"/> indicating the outcome of the operation.
        /// </returns>
        public async Task<ServiceResult> UpdateAddressAsync(int businessId, AddressUpdateDto dto)
        {
            var spec = new BusinessSpecifications
            {
                IncludeAddress = true
            };

            var business = await _businessRepository.GetByIdAsync(businessId, spec);
            if (business == null)
            {
                _logger.LogWarning("Business with ID {BusinessId} not found during address update.", businessId);
                return ServiceResult.Failure("Business not found.", 404);
            }

            if (business.Address == null)
            {
                business.Address = new Address();
            }

            _mapper.Map(dto, business.Address);
            await _businessRepository.UpdateAsync(business);

            _logger.LogInformation("Address updated for business {BusinessId}.", businessId);
            return ServiceResult.SuccessResult();
        }


        /// <summary>
        /// Updates booking-related settings for the specified business,
        /// including auto-confirmation and buffer time between bookings.
        /// </summary>
        /// <param name="businessId">The ID of the business.</param>
        /// <param name="dto">Settings data to apply.</param>
        /// <returns>A <see cref="ServiceResult"/> indicating success or failure.</returns>
        public async Task<ServiceResult> UpdateSettingsAsync(int businessId, BusinessSettingsUpdateDto dto)
        {
            var spec = new BusinessSpecifications
            {
                IncludeSettings = true
            };

            var business = await _businessRepository.GetByIdAsync(businessId, spec);
            if (business == null)
            {
                _logger.LogWarning("Business {BusinessId} not found for settings update.", businessId);
                return ServiceResult.Failure("Business not found.", 404);
            }

            if (business.Settings == null)
            {
                business.Settings = new BusinessSettings();
            }

            if (dto.AutoConfirmBookings != null)
                business.Settings.AutoConfirmBookings = dto.AutoConfirmBookings.Value;

            if (dto.BookingBufferTime != null)
                business.Settings.BookingBufferTime = dto.BookingBufferTime.Value;

            await _businessRepository.UpdateAsync(business);

            _logger.LogInformation("Updated settings for business {BusinessId}. Fields changed: {Fields}", businessId, new
            {
                AutoConfirmBookings = dto.AutoConfirmBookings != null,
                BookingBufferTime = dto.BookingBufferTime != null
            });

            return ServiceResult.SuccessResult();
        }

        /// <summary>
        /// Retrieves a paginated list of published businesses based on the provided filters and pagination settings.
        /// </summary>
        /// <param name="query">Filter and pagination parameters.</param>
        /// <returns>
        /// A <see cref="ServiceResult{BusinessListDto}"/> containing the list of businesses and pagination metadata.
        /// </returns>
        public async Task<ServiceResult<BusinessListDto>> GetPaginatedBusinessesAsync(BusinessQueryParams query)
        {
            _logger.LogInformation("Fetching businesses from repository. Page: {Page}, PageSize: {Size}, CategoryId: {CategoryId}, City: {City}, SearchTerms: {SearchTerms}",
                query.Page, query.PageSize, query.CategoryId, query.City, query.SearchTerms);

            var totalRecords = await _businessRepository.CountBusinessesAsync(query.CategoryId, query.City, query.SearchTerms);

            var businesses = await _businessRepository.GetPaginatedBusinessesAsync(query.Page, query.PageSize, query.CategoryId, query.City, query.SearchTerms);

            var businessDtos = _mapper.Map<List<BusinessListItemDto>>(businesses);

            var businessListDto = new BusinessListDto
            {
                BusinessList = businessDtos,
                CurrentPage = query.Page,
                TotalPages = (int)Math.Ceiling((double)totalRecords / query.PageSize),
                TotalRecords = totalRecords
            };

            _logger.LogInformation("Successfully retrieved {Count} businesses. Total records: {Total}", businessDtos.Count, totalRecords);

            return ServiceResult<BusinessListDto>.SuccessResult(businessListDto);
        }

        /// <summary>
        /// Retrieves a single published business by ID with full public details.
        /// </summary>
        /// <param name="businessId">The ID of the business to retrieve.</param>
        /// <returns>
        /// A <see cref="ServiceResult{BusinessDto}"/> containing the business if found;
        /// or 404 if not published or not found.
        /// </returns>
        public async Task<ServiceResult<BusinessPublicDetailsDto>> GetPublicBusinessByIdAsync(int businessId, int? userId)
        {
            var spec = new BusinessSpecifications
            {
                IncludeAddress = true,
                IncludeRegistration = true,
                IncludeImages = true,
                IncludeSchedule = true,
                IncludeEmployees = true,
                IncludeServices = true,
                IncludeReviews = true
            };

            var business = await _businessRepository.GetByIdAsync(businessId, spec);

            if (business == null || !business.IsPublished)
            {
                _logger.LogWarning("Business {BusinessId} not found or not published.", businessId);
                return ServiceResult<BusinessPublicDetailsDto>.Failure("Business not found or not published.", 404);
            }

            var businessDto = _mapper.Map<BusinessPublicDetailsDto>(business);

            if (userId is not null)
            {
                var review = await _reviewRepostiory.GetByUserAndBusinessAsync(userId.Value, businessId);
                if (review != null)
                {
                    businessDto.ExitingReviewId = review.Id;
                }
                else
                {
                    businessDto.CanReview = await _bookingService.UserHasCompleteBooking(userId.Value, businessId);
                }
            }

            return ServiceResult<BusinessPublicDetailsDto>.SuccessResult(businessDto);
        }
    }
}
