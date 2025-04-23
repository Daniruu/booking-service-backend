using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class UserUpdateDto
    {
        [MaxLength(50)]
        public string? Name { get; set; }

        [MaxLength(50)]
        public string? Surname { get; set; }
    }
}
