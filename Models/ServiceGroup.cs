using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class ServiceGroup
    {
        public int Id { get; set; }

        public int BusinessId { get; set; }
        public Business Business { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        public string? IconUrl { get; set; }

        public int Order { get; set; }
        public bool IsSystem { get; set; }
        public List<Service> Services { get; set; } = new();
    }
}
