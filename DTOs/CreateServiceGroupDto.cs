using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class CreateServiceGroupDto
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }
    }
}
