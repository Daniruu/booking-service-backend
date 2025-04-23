using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class ChangeEmailRequestDto
    {
        [Required, EmailAddress]
        public string NewEmail { get; set; }
    }
}
