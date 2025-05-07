using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class UpdateAddressDto
    {
        [Required]
        public string PostalCode { get; set; }

        [Required, MaxLength(100)]
        public string Country { get; set; }

        [Required]
        public string Region { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string BuildingNumber { get; set; }

        public string? RoomNumber { get; set; }
    }
}
