using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class BusinessRegistrationDataUpdateDto
    {
        [MaxLength(10)]
        public string? Nip { get; set; }

        [MaxLength(14)]
        public string? Regon { get; set; }

        public string? Krs { get; set; }
    }
}
