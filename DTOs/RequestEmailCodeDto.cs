using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class RequestEmailCodeDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
