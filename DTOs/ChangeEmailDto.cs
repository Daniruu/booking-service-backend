using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class ChangeEmailDto
    {
        [Required, EmailAddress]
        public string NewEmail { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
