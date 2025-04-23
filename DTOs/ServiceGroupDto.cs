using BookingService.Models;

namespace BookingService.DTOs
{
    public class ServiceGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool IsSystem { get; set; }
        public List<ServiceDto> Services { get; set; }
    }
}
