using BCrypt.Net;

namespace BookingService.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public PasswordHasher() { }

        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));
            }
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordHash))
            {
                throw new ArgumentException("Password and hash cannot be null or empty.");
            }
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
