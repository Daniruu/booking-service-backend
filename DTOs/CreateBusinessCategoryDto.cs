using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class CreateBusinessCategoryDto
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Required]
        public IFormFile Icon { get; set; }
    }
}
