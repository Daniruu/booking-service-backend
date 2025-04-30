using BookingService.Models;
using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class EmailCodeVerificationDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }
    }
}
