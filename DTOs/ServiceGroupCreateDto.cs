using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class ServiceGroupCreateDto
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }
    }
}
