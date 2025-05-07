using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class PatchBusinessSettingsDto
    {
        public bool? AutoConfirmBookings { get; set; }

        public TimeSpan? BookingBufferTime { get; set; }
    }
}
