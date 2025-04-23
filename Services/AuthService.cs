using BookingService.Data;
using BookingService.Repositories;
using BookingService.Utils;

namespace BookingService.Services
{
    /// <summary>
    /// Handles authentication logic including user login, token refresh, and logout.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly BookingServiceDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ILogger<AuthService> _logger;
        public AuthService(
            BookingServiceDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IAccountRepository accountRepository,
            IPasswordHasher passwordHasher,
            ITokenGenerator tokenGenerator,
            ILogger<AuthService> logger
            )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _accountRepository = accountRepository;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a user using email and password. If successful, returns an access token and sets a refresh token cookie.
        /// </summary>
        /// <param name="email">User's email address.</param>
        /// <param name="password">User's password.</param>
        /// <returns>Access token or error result.</returns>
        public async Task<ServiceResult<string>> AuthenticateUserAsync(string email, string password)
        {
            _logger.LogInformation("Authenticating user: {Email}", email);

            email = email.Trim().ToLower();

            var account = await _accountRepository.GetByEmailAsync(email);
            if (account == null)
            {
                _logger.LogWarning("Authentication failed: user not found ({Email})", email);
                return ServiceResult<string>.Failure("Invalid email or password.", 401);
            }

            if (!_passwordHasher.VerifyPassword(password, account.PasswordHash))
            {
                _logger.LogWarning("Authentication failed: invalid password ({Email})", email);
                return ServiceResult<string>.Failure("Invalid email or password.", 401);
            }

            var accessToken = _tokenGenerator.GenerateToken(account);
            var refreshToken = await _tokenGenerator.GenerateRefreshToken();

            account.RefreshToken = refreshToken;
            account.RefreshExpiryTime = DateTimeOffset.UtcNow.AddDays(7);
            await _accountRepository.Update(account);
            await _context.SaveChangesAsync();

            var response = _httpContextAccessor.HttpContext.Response;

            response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            _logger.LogInformation("Authentication successful: {Email}", email);
            return ServiceResult<string>.SuccessResult(accessToken);
        }

        /// <summary>
        /// Refreshes the access token using a valid refresh token from the cookie.
        /// </summary>
        /// <param name="refreshToken">Refresh token stored in HTTP-only cookie.</param>
        /// <returns>New access token or error result.</returns>
        public async Task<ServiceResult<string>> RefreshTokenAsync(string refreshToken)
        {
            _logger.LogInformation("Token refresh attempt using refresh token: {Token}", refreshToken);

            var account = await _accountRepository.GetUserByRefreshTokenAsync(refreshToken);
            if (account == null || account.RefreshToken != refreshToken || account.RefreshExpiryTime <= DateTime.UtcNow)
            {
                _logger.LogWarning("Invalid or expired refresh token.");
                return ServiceResult<string>.Failure("Invalid or expired refresh token.", 401);
            }

            var newAccessToken = _tokenGenerator.GenerateToken(account);
            var newRefreshToken = await _tokenGenerator.GenerateRefreshToken();

            account.RefreshToken = newRefreshToken;
            account.RefreshExpiryTime = DateTimeOffset.UtcNow.AddDays(7);
            await _accountRepository.Update(account);
            await _context.SaveChangesAsync();

            var response = _httpContextAccessor.HttpContext?.Response;

            response?.Cookies.Append("RefreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            _logger.LogInformation("Access and refresh tokens successfully refreshed for account ID {Id}", account.Id);
            return ServiceResult<string>.SuccessResult(newAccessToken);
        }

        /// <summary>
        /// Logs the user out by invalidating the refresh token and removing the cookie.
        /// </summary>
        /// <param name="refreshToken">Current refresh token from the cookie.</param>
        /// <returns>Success or error result.</returns>
        public async Task<ServiceResult> LogoutAsync(string refreshToken)
        {
            _logger.LogInformation("Logout attempt with refresh token: {Token}", refreshToken);

            var account = await _accountRepository.GetUserByRefreshTokenAsync(refreshToken);
            if (account == null)
            {
                _logger.LogWarning("Logout failed: account not found for refresh token");
                return ServiceResult.Failure("Invalid refresh token.", 400);
            }

            account.RefreshToken = null;
            account.RefreshExpiryTime = null;
            await _accountRepository.Update(account);
            await _context.SaveChangesAsync();

            var response = _httpContextAccessor.HttpContext?.Response;

            response?.Cookies.Delete("RefreshToken");

            _logger.LogInformation("Logout successful for account ID {Id}", account.Id);
            return ServiceResult.SuccessResult();
        }
    }
}
