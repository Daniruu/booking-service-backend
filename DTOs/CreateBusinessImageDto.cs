using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class CreateBusinessImageDto
    {
        [Required]
        public IFormFile File { get; set; }
        public string? AltText { get; set; }
    }
}
