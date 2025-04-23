using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required, MaxLength(500)]
        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        [Precision(18, 2)]
        public decimal Price { get; set; }

        public TimeSpan Duration { get; set; }

        public int Order { get; set; }

        public bool IsFeatured { get; set; }

        public int BusinessId { get; set; }
        public Business Business { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int ServiceGroupId { get; set; }
        public ServiceGroup ServiceGroup { get; set; }
    }
}
