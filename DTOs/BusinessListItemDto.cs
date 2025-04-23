using BookingService.Models;

namespace BookingService.DTOs
{
    public class BusinessListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string Email { get; set; }
        public BusinessImageDto Image { get; set; }
        public Address Address { get; set; }

        public double AverageRating { get; set; }
    }
}
