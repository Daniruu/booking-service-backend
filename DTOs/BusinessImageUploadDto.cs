using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class BusinessImageUploadDto
    {
        [Required]
        public IFormFile File { get; set; }
        public string? AltText { get; set; }
    }
}
