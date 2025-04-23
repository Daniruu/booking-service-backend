using AutoMapper;
using BookingService.Data;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Utils;

namespace BookingService.Services
{
    /// <summary>
    /// Handles the registration process for user and business accounts.
    /// Validates email, confirmation code, and business category (if applicable).
    /// </summary>
    public class RegistrationService : IRegistrationService
    {
        private readonly BookingServiceDbContext _context;
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;
        private readonly IConfirmationCodeService _confirmationCodeService;
        private readonly IBusinessCategoryRespository _businessCategoryRespository;
        private readonly ILogger<RegistrationService> _logger;
        public RegistrationService(
            BookingServiceDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IAccountRepository accountRepository,
            IUserRepository userRepository,
            IBusinessRepository businessRepository,
            IPasswordHasher passwordHasher,
            IEmailSender emailSender,
            IMapper mapper,
            IConfirmationCodeService confirmationCodeService,
            ITokenGenerator tokenGenerator,
            IBusinessCategoryRespository businessCategoryRepository,
            ILogger<RegistrationService> logger
            )
        {
            _context = context;
            _accountRepository = accountRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _confirmationCodeService = confirmationCodeService;
            _businessCategoryRespository = businessCategoryRepository;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user account after verifying email and confirmation code.
        /// </summary>
        /// <param name="dto">Data required to register a user, including email, password, phone number, and confirmation code.</param>
        /// <returns>ID of the newly created user or an error.</returns>
        public async Task<ServiceResult<int>> RegisterUserAsync(RegisterUserDto dto)
        {
            _logger.LogInformation("Attempting to register user: {Email}", dto.Email);

            dto.Email = dto.Email.Trim().ToLower();
            dto.Phone = dto.Phone.Trim();

            if (await _accountRepository.EmailExistsAsync(dto.Email))
            {
                _logger.LogWarning("Email already exists: {Email}", dto.Email);
                return ServiceResult<int>.Failure("Email is already registered.", 409);
            }

            var verification = await _confirmationCodeService.VerifyAsync(dto.Email, dto.Code, ConfirmationCodeType.EmailConfirmaiton);
            if (!verification.Success)
            {
                _logger.LogWarning("Email confirmation failed for {Email}", dto.Email);
                return ServiceResult<int>.Failure(verification.ErrorMessage!, verification.StatusCode);
            }

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = _passwordHasher.HashPassword(dto.Password);

            await _accountRepository.AddAsync(user);

            _logger.LogInformation("User registered successfully: {UserId}", user.Id);
            return ServiceResult<int>.SuccessResult(user.Id);
        }

        /// <summary>
        /// Registers a new business account after verifying email, confirmation code, and business category.
        /// </summary>
        /// <param name="dto">Data required to register a business, including category ID, email, password, and confirmation code.</param>
        /// <returns>ID of the newly created business or an error.</returns>
        public async Task<ServiceResult<int>> RegisterBusinessAsync(RegisterBusinessDto dto)
        {
            _logger.LogInformation("Attempting to register business: {Email}, CategoryId: {CategoryId}", dto.Email, dto.CategoryId);

            dto.Email = dto.Email.Trim().ToLower();
            dto.Phone = dto.Phone.Trim();

            if (await _accountRepository.EmailExistsAsync(dto.Email))
            {
                _logger.LogWarning("Email already exists: {Email}", dto.Email);
                return ServiceResult<int>.Failure("Email is already registered.", 409);
            }

            var verification = await _confirmationCodeService.VerifyAsync(dto.Email, dto.Code, ConfirmationCodeType.EmailConfirmaiton);
            if (!verification.Success)
            {
                _logger.LogWarning("Email confirmation failed: {Email}", dto.Email);
                return ServiceResult<int>.Failure(verification.ErrorMessage!, verification.StatusCode);
            }

            if (await _businessCategoryRespository.GetByIdAsync(dto.CategoryId) == null)
            {
                _logger.LogWarning("Invalid business category: {CategoryId}", dto.CategoryId);
                return ServiceResult<int>.Failure("Invalid category.", 400);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var business = _mapper.Map<Business>(dto);
                business.PasswordHash = _passwordHasher.HashPassword(dto.Password);

                await _accountRepository.AddAsync(business);
                await _context.SaveChangesAsync();

                var systemGroup = new ServiceGroup
                {
                    BusinessId = business.Id,
                    Name = "Inne",
                    Order = 1,
                    IsSystem = true,
                };

                await _context.ServiceGroups.AddAsync(systemGroup);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation("Business registered successfully: {BusinessId}", business.Id);
                return ServiceResult<int>.SuccessResult(business.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error during business registration: {Email}", dto.Email);
                return ServiceResult<int>.Failure($"Business registration failed: {ex.Message}", 500);
            }
        }
    }
}
