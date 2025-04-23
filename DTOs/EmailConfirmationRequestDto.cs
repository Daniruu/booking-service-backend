using BookingService.Models;
using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class EmailConfirmationRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public ConfirmationCodeType Type { get; set; }
    }
}
