using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class Business : Account
    {
        [Required]
        public int CategoryId { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool IsPublished { get; set; } = false;

        public BusinessRegistrationData RegistrationData { get; set; }

        public Address Address { get; set; }

        [Required]
        public BusinessSettings Settings { get; set; } = new();

        public List<DaySchedule> Schedule { get; set; } = new();
        public List<BusinessImage> Images { get; set; } = new();
        public List<Employee> Employees { get; set; } = new();
        public List<ServiceGroup> ServiceGroups { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        public List<Booking> Bookings { get; set; }
    }
}
