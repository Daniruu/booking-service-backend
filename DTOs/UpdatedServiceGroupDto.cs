namespace BookingService.DTOs
{
    public class UpdatedServiceGroupDto
    {
        public bool IsGroupChanged { get; set; }
        public List<ServiceDto> OldGroupServices { get; set; }
        public List<ServiceDto> NewGroupServices { get; set; }
    }
}
