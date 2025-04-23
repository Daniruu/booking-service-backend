using System.ComponentModel.DataAnnotations;

namespace BookingService.Models
{
    public class BusinessRegistrationData
    {
        [Required, MaxLength(10)]
        public string Nip { get; set; }

        [Required, MaxLength(14)]
        public string Regon { get; set; }

        public string? Krs { get; set; }
    }
}
