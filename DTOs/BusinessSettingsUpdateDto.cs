using System.ComponentModel.DataAnnotations;

namespace BookingService.DTOs
{
    public class BusinessSettingsUpdateDto
    {
        public bool? AutoConfirmBookings { get; set; }

        public TimeSpan? BookingBufferTime { get; set; }
    }
}
