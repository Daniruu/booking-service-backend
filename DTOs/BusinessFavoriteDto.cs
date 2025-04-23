using BookingService.Models;

namespace BookingService.DTOs
{
    public class BusinessFavoriteDto
    {
        public int UserId { get; set; }
        public int BusinessId { get; set; }
        public string BusinessName { get; set; }
        public string BusinessPhone { get; set; }
        public string BusinessEmail { get; set; }
        public string BusinessDescription { get; set; }
        public int BusinessCategoryId { get; set; }
        public Address BusinessAddress { get; set; }
        public string BusinessImage { get; set; }
    }
}
