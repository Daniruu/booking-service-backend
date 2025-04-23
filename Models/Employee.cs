using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public int BusinessId { get; set; }
        public Business Business { get; set; }

        [Required]
        public string Position { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, Phone]
        public string Phone { get; set; }

        public string? AvatarUrl { get; set; }

        public List<DaySchedule> Schedule { get; set; } = new();
    }
}
