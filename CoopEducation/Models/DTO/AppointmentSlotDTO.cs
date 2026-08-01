namespace CoopEducation.Models.DTO
{
    public class AppointmentSlotDTO
    {
        public int TeacherId { get; set; }
        public DateOnly AvailableDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string SupervisionModel { get; set; } = null!;
        public string? Location { get; set; }
        public string? Remark { get; set; }
        public int? MaxStudent { get; set; }
    }
}
