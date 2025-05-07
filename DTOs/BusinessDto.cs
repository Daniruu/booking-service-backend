using BookingService.Models;

namespace BookingService.DTOs
{
    public class BusinessDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public BusinessRegistrationData RegistrationData { get; set; }
        public Address Address { get; set; }
        public BusinessSettings Settings { get; set; }
    }
}
