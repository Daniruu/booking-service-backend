using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class PasswordRecoveryRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
