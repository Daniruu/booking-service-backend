using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class RegisterUserDto
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required, MaxLength(50)]
        public string Surname { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, Phone]
        public string Phone { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
