using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class BusinessCategory
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }

        [Required]
        public string IconUrl { get; set; }
    }
}
