using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class BusinessCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string IconUrl { get; set; }
    }
}
