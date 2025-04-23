using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class ServiceCreateDto
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required, MaxLength(500)]
        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        public TimeSpan Duration { get; set; }

        public int EmployeeId { get; set; }

        public int ServiceGroupId { get; set; }
    }
}
