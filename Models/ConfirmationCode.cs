using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace BookingService.Models
{
    public enum ConfirmationCodeType
    {
        EmailConfirmaiton,
        PasswordRecovery,
        EmailChange
    }
    public class ConfirmationCode
    {
        [Key]
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public ConfirmationCodeType CodeType { get; set; }
        public DateTimeOffset ExpirationTime { get; set; }
        public DateTimeOffset LastRequestTime { get; set; }

        public static string Generate(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            using var rng = RandomNumberGenerator.Create();
            var buffer = new byte[length];
            rng.GetBytes(buffer);
            return new string(buffer.Select(b => chars[b % chars.Length]).ToArray());
        }
    }
}
