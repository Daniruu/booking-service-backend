namespace BookingService.DTOs
{
    public class ServiceGroupUpdatedDto
    {
        public bool IsGroupChanged { get; set; }
        public List<ServiceDto> OldGroupServices { get; set; }
        public List<ServiceDto> NewGroupServices { get; set; }
    }
}
