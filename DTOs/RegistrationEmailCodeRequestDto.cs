using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class RegistrationEmailCodeRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
