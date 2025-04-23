using System.Security.Claims;

namespace BookingService.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var claims = user.FindFirst(ClaimTypes.NameIdentifier)
                        ?? throw new UnauthorizedAccessException("User ID claim is missings.");

            return int.Parse(claims.Value);
        }
        public static bool TryGetUserId(this ClaimsPrincipal user, out int userId)
        {
            userId = 0;

            if (user?.Identity?.IsAuthenticated != true)
                return false;

            var claims = user.FindFirst(ClaimTypes.NameIdentifier);
            return claims != null && int.TryParse(claims.Value, out userId);
        }

        public static string? GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static string? GetUserRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}
