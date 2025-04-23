namespace BookingService.Specifications
{
    public class BusinessSpecifications
    {
        public bool IncludeAddress { get; set; } = false;
        public bool IncludeRegistration { get; set; } = false;
        public bool IncludeSettings { get; set; } = false;
        public bool IncludeSchedule { get; set; } = false;
        public bool IncludeImages { get; set; } = false;
        public bool IncludeEmployees { get; set; } = false;
        public bool IncludeServices { get; set; } = false;
        public bool IncludeReviews { get; set; } = false;
        public bool IncludeBookings { get; set; } = false;
    }
}
