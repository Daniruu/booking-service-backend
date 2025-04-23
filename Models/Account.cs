using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public enum Roles
    {
        User,
        Business,
        Admin
    }

    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Roles Role { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, Phone]
        public string Phone { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [MaxLength(255)]
        public string? RefreshToken { get; set; }

        public DateTimeOffset? RefreshExpiryTime { get; set; }
    }
}
